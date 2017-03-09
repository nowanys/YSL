using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YSL.Model;
using Microsoft.EntityFrameworkCore;
using YSL.Common;

namespace YSL.Controllers.Hrm
{
    public class EmployeeController : Controller
    {
        /// <summary>
        /// �������޸�Ա����Ϣ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IActionResult SaveEmployee(hrm_employee employee)
        {
            //todo:�������֤�ţ���ȡ���գ��ͻ���У�����֤���Ƿ�Ϸ���
            var addFlag = false;
            if (string.IsNullOrEmpty(employee.id))
            {
                employee.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            var db = YSLContextFactory.Create();
            try
            {
                db.Entry(employee).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch
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
        public IActionResult DelAccount(string id)
        {
            var target = new hrm_employee() { id = id };
            var db = YSLContextFactory.Create();
            try
            {
                db.hrm_employee.Attach(target);
                db.hrm_employee.Remove(target);
                db.SaveChanges();
            }
            catch
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
        public IActionResult GetEmployeeById(string id)
        {
            var db = YSLContextFactory.Create();
            hrm_employee result;
            try
            {
                result = db.hrm_employee.Where(m => m.id == id).FirstOrDefault();
            }
            catch
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