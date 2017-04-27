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
    /// Ա��������
    /// </summary>
    public class EmployeeController : Controller
    {
        /// <summary>
        /// ��ȡԱ����Ϣ
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetEmployeeByPage([FromBody]JObject form)
        {
            var page = form["pager"].ToObject<PageDataRequestModel>();
            var searchTxt = form["searchTxt"] == null ? "" : form["searchTxt"].ToString();
            List<hrm_employee> data;
            int rowCount = 0;
            var db = new YSLContext();
            try
            {
                var query = db.hrm_employee
                    .Where(m => m.employee_name.Contains(searchTxt))
                    .OrderByDescending(m => m.add_time);
                rowCount = query.Count();
                data = query.Skip(page.page_index * page.page_size)
                    .Take(page.page_size).ToList();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("��ȡ����ϵͳȨ���쳣��");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(rowCount, data);
            return result;
        }
        /// <summary>
        /// �������޸�Ա����Ϣ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IActionResult SaveEmployee([FromBody]hrm_employee employee)
        {
            //todo:�������֤�ţ���ȡ���գ��ͻ���У�����֤���Ƿ�Ϸ���
            var addFlag = false;
            if (string.IsNullOrEmpty(employee.id))
            {
                employee.id = Guid.NewGuid().ToString("N");
                employee.add_time = DateTime.Now;
                addFlag = true;
            }
            var db = new YSLContext();
            try
            {
                db.Entry(employee).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return ResultToJson.ToError("�������޸�Ա����Ϣʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ɾ��һ��Ա��������ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelEmployee([FromBody] hrm_employee target)
        {
            var db = new YSLContext();
            try
            {
                db.hrm_employee.Attach(target);
                db.hrm_employee.Remove(target);
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
        /// <summary>
        /// ����Ա��ID����ȡԱ����Ϣ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetEmployeeById([FromBody] hrm_employee target)
        {
            var db = new YSLContext();
            hrm_employee result;
            try
            {
                result = db.hrm_employee.Where(m => m.id == target.id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("ɾ���˻�ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess(result);
        }

    }
}