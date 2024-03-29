﻿using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Sieve.Models;
using Sieve.Services;
using System.Net;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class FAQCategoryController : BaseController
    {
        private readonly IFAQCategoryRepo _faqCategoryRepo;
        private readonly IMapper _mapper;
        private readonly ISieveProcessor _sieveProcessor;
        private readonly SieveOptions _sieveOptions;
        public FAQCategoryController(IFAQCategoryRepo faqRepo, IMapper mapper, ISieveProcessor sieveProcessor, IOptions<SieveOptions> sieveOptions)
        {
            _faqCategoryRepo = faqRepo;
            _mapper = mapper;
            _sieveProcessor = sieveProcessor;
            _sieveOptions = sieveOptions?.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] SieveModel sieveModel)
        {
            try
            {
				IEnumerable<FAQCategory> faqsCategories = await _faqCategoryRepo.GetAllAsync(true, e => e.FAQS);
				List<FAQCategoryGetDTO> result = new List<FAQCategoryGetDTO>();
				foreach (var faqCategory in faqsCategories)
					result.Add(new FAQCategoryGetDTO() { Id = faqCategory.Id, Name = faqCategory.Name, FAQS = _mapper.Map<List<FAQGetDTO>>(faqCategory.FAQS) });
                var FilteredFAQ = _sieveProcessor.Apply(sieveModel, result.AsQueryable());

                return CustomResult(FilteredFAQ);
			}
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
	        }
        }
       
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            if (id == 0) 
                return CustomResult(HttpStatusCode.BadRequest);
            try
            {
				FAQCategory faqsCategory = await _faqCategoryRepo.GetCategoryByIdWithFAQ(id);
				if (faqsCategory == null)
					return CustomResult(HttpStatusCode.NotFound);
				FAQCategoryGetDTO result = new FAQCategoryGetDTO() { Id = faqsCategory.Id, Name = faqsCategory.Name, FAQS = _mapper.Map<List<FAQGetDTO>>(faqsCategory.FAQS) };
				return CustomResult(result);
			}
            catch
            {
				return CustomResult(HttpStatusCode.InternalServerError);
			}
            
        }
        
        [HttpPost]
        public async Task<IActionResult> Add(FAQCategoryAddDTO faq)
        {
            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest); 

            var faqCategoryExists = await _faqCategoryRepo.FindByName(faq.Name);
            if (faqCategoryExists != null)
                return CustomResult("This Category Already Exists", HttpStatusCode.BadRequest);

            var result = await _faqCategoryRepo.AddAsync(_mapper.Map<FAQCategory>(faq));
            return CustomResult(result);

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id==0) 
                return CustomResult(HttpStatusCode.BadRequest);
            try
            {
                var tryExist = _faqCategoryRepo.GetByIdAsync(id);
                if (tryExist == null) return CustomResult(HttpStatusCode.NotFound);
                await _faqCategoryRepo.SoftDeleteAsync(id);
                return CustomResult();
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id,FAQCategoryAddDTO faq)
        {
            if (id == 0)
                return CustomResult(HttpStatusCode.BadRequest);
            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            try
            {
                var faqCategoryFromDB = await _faqCategoryRepo.GetByIdAsync(id);
                if (faqCategoryFromDB == null) 
                    return CustomResult(HttpStatusCode.BadRequest);

                var result = await _faqCategoryRepo.EditAsync(id, _mapper.Map<FAQCategory>(faq),e=>e.Id) ;
                return CustomResult(result);
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}
