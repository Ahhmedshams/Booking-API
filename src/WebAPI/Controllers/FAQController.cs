using Application.Common.Interfaces.Repositories;
using AutoMapper;
using CoreApiResponse;
using Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Collections;
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
            if (id == 0) return CustomResult(HttpStatusCode.BadRequest);
            try
            {
                FAQ faq = await _faqRepo.GetByIdAsync(id);
                if (faq == null) return CustomResult(HttpStatusCode.NotFound);
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
                if (!ModelState.IsValid) return CustomResult(ModelState, HttpStatusCode.BadRequest);
                var faqExists = await _faqRepo.FindByQuestion(faq.Question);
                if (faqExists!=null) return BadRequest("This Question Already Exists");
                var categoryExists = await _faqRepo.CategoryExits(faq.FAQCategoryId);
                if(!categoryExists) return BadRequest("This Category Doesn't Exists");
                var result = await _faqRepo.AddAsync(_mapper.Map<FAQ>(faq));
                return CustomResult(result);
            }
            catch
            {
                return CustomResult(HttpStatusCode.InternalServerError);
            }

        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            if(id==0) return BadRequest();
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
        public async Task<IActionResult> Update(int id,FAQAddDTO faq)
        {
            if (id == 0) return BadRequest();
            if (!ModelState.IsValid) return CustomResult(ModelState, HttpStatusCode.BadRequest);
            try
            {
                var faqFromDB = await _faqRepo.GetByIdAsync(id);
                if (faqFromDB ==null) return BadRequest();
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
