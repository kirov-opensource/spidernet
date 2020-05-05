using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Spidernet.BLL.Models.Templates;
using Spidernet.DAL.Repositories;
using Spidernet.Extension.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Spidernet.BLL.Services {
  /// <summary>
  /// 
  /// </summary>
  public class TemplateService : Service {
    private readonly TemplateRepository templateRepository;
    /// <summary>
    /// 
    /// </summary>
    /// <param name="loggerFactory"></param>
    /// <param name="session"></param>
    /// <param name="templateRepository"></param>
    public TemplateService(ILoggerFactory loggerFactory, Session session, TemplateRepository templateRepository) : base(loggerFactory, null, session) {
      this.templateRepository = templateRepository;
    }


    /// <summary>
    /// 根据编号查询
    /// </summary>
    /// <param name="no"></param>
    /// <returns></returns>
    public async Task<TemplateDetailViewModel> GetByNo(string no) {
      var quriedTemplate = await this.templateRepository.GetByNo(no);
      if (quriedTemplate != null) {
        return new TemplateDetailViewModel {
          CreateAt = quriedTemplate.create_at,
          Id = quriedTemplate.id,
          Name = quriedTemplate.name,
          No = quriedTemplate.no,
          PropertyParsingRule = quriedTemplate.property_parsing_rule,
          UpdateAt = quriedTemplate.update_at,
          Uri = quriedTemplate.uri
        };
      } else {
        return null;
      }
    }

    public async Task Update(string no, UpdateTemplateModel updateTemplateModel) {
      var originTemplate = await templateRepository.GetByNo(no);
      if (originTemplate == null)
        throw new BusinessException("模板不存在");

      originTemplate.name = updateTemplateModel.Name;
      originTemplate.property_parsing_rule = updateTemplateModel.PropertyParsingRule;
      originTemplate.uri = updateTemplateModel.Uri;

      await templateRepository.UpdateAsync(originTemplate);
    }

    /// <summary>
    /// 创建Template
    /// </summary>
    /// <param name="createTemplateModel"></param>
    /// <returns></returns>
    public async Task Create(CreateTemplateModel createTemplateModel) {
      var insertEntity = new DAL.Entities.Template {
        name = createTemplateModel.Name,
        //header = new Newtonsoft.Json.Linq.JObject { },
        no = createTemplateModel.No,
        property_parsing_rule = createTemplateModel.PropertyParsingRule,
        //subsequent_task_property_scheme = new Newtonsoft.Json.Linq.JObject { },
        uri = createTemplateModel.Uri,
        user_id = session.User.Id
      };
      await this.templateRepository.InsertAsync(insertEntity);
    }
  }
}
