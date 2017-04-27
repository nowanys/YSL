using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YSL.Model;
using Microsoft.EntityFrameworkCore;
using YSL.Common;
using Newtonsoft.Json.Linq;

namespace YSL.Controllers.Hrm
{
    /// <summary>
    /// ��ѵ������
    /// </summary>
    public class TrainController : Controller
    {
        /// <summary>
        /// ��ȡ��ѵ�б�
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetTrainByPage([FromBody]JObject form)
        {
            var page = form["pager"].ToObject<PageDataRequestModel>();
            var searchTxt = form["searchTxt"] == null ? "" : form["searchTxt"].ToString();
            List<hrm_train> data;
            int rowCount = 0;
            var db = new YSLContext();
            try
            {
                var query = db.hrm_train
                    .Where(m => m.train_title.Contains(searchTxt))
                    .OrderByDescending(m => m.begin_time);
                rowCount = query.Count();
                data = query.Skip(page.page_index * page.page_size)
                    .Take(page.page_size).ToList();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("��ȡ��ѵ�б��쳣��");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(rowCount, data);
            return result;
        }
        /// <summary>
        /// �������޸���ѵ��Ŀ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IActionResult SaveEmployee([FromBody]hrm_train train)
        {
            var addFlag = false;
            if (string.IsNullOrEmpty(train.id))
            {
                train.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            var db = new YSLContext();
            try
            {
                db.Entry(train).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return ResultToJson.ToError("�������޸���ѵ��Ŀʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ɾ��һ����ѵ��Ŀ������ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelAccount([FromBody]hrm_train target)
        {
            var db = new YSLContext();
            try
            {
                db.hrm_train.Attach(target);
                db.hrm_train.Remove(target);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("ɾ���˻�ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
    }
}