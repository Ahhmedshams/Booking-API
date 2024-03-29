﻿using AutoMapper;
using CoreApiResponse;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace WebAPI.Controllers
{
	[Route("api/[controller]")]
    [ApiController]
    public class FAQController : BaseController
    {
        private readonly IFAQRepo _faqRepo;
        private readonly IMapper _mapper;

        public FAQController(IFAQRepo faqRepo, IMapper mapper)
        {
            _faqRepo = faqRepo;
            _mapper = mapper;
        }


        [HttpGet]
        public async Task<IActionResult> GetAll(){
            try
            {
                IEnumerable<FAQ> faqs=await _faqRepo.GetAllAsync();
                return CustomResult(faqs);
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
                FAQ faq = await _faqRepo.GetByIdAsync(id);
                return CustomResult(faq);
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(FAQAddDTO faq)
        {
            try
            {
                if (!ModelState.IsValid) 
                    return CustomResult(ModelState, HttpStatusCode.BadRequest);

                var faqExists = await _faqRepo.FindByQuestion(faq.Question);
                if (faqExists!=null) 
                    return CustomResult("This Question Already Exists", HttpStatusCode.BadRequest);

                var categoryExists = await _faqRepo.CategoryExits(faq.FAQCategoryId);
                if(!categoryExists) 
                    return CustomResult("This Category Doesn't Exists", HttpStatusCode.BadRequest);

                var result = await _faqRepo.AddAsync(_mapper.Map<FAQ>(faq));
                return CustomResult(result);
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }

        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            if(id==0) 
                return CustomResult(HttpStatusCode.BadRequest);
            try
            {
                await _faqRepo.SoftDeleteAsync(id);
                return CustomResult();
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }
        }
        
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update([FromRoute]int id,FAQAddDTO faq)
        {
            if (id == 0) 
                return CustomResult(HttpStatusCode.BadRequest);

            if (!ModelState.IsValid) 
                return CustomResult(ModelState, HttpStatusCode.BadRequest);
            try
            {
                var faqFromDB = await _faqRepo.GetByIdAsync(id);
                if (faqFromDB ==null) 
                    return CustomResult(HttpStatusCode.BadRequest);

                var result = await _faqRepo.EditAsync(id, _mapper.Map<FAQ>(faq),e=>e.Id) ;
                return CustomResult(result);
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }
        }
    }
}
