using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Spidernet.BLL;
using Spidernet.BLL.Models.Templates;
using Spidernet.BLL.Services;

namespace Spidernet.WebAPI.Controllers.V1 {
  /// <summary>
  /// 模板模块
  /// </summary>
  public class TemplateController : BaseController {

    private readonly TemplateService templateService;

    public TemplateController(ILoggerFactory logger, Session session, TemplateService templateService) : base(logger, session) {
      this.templateService = templateService;
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="no"></param>
    /// <param name="updateTemplateModel"></param>
    /// <returns></returns>
    [HttpPut("{no}")]
    public async Task<IActionResult> Update(string no, UpdateTemplateModel updateTemplateModel) {
      await templateService.Update(no, updateTemplateModel);
      return NoContent();
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    [HttpGet("{no}")]
    public async Task<IActionResult> Get(string no) {
      var result = await templateService.GetByNo(no);
      return Ok(result);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="createTemplateModel"></param>
    /// <returns></returns>
    [HttpPost]
    public async Task<IActionResult> Create(CreateTemplateModel createTemplateModel) {
      await this.templateService.Create(createTemplateModel);
      return NoContent();
    }
  }
}