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
    public class DepartmentController : Controller
    {
        /// <summary>
        /// �������޸Ĳ�����Ϣ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IActionResult SaveDepartment(hrm_department obj)
        {
            var addFlag = false;
            if (string.IsNullOrEmpty(obj.id))
            {
                obj.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            var db = YSLContextFactory.Create();
            try
            {
                db.Entry(obj).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("�������޸Ĳ�����Ϣʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ɾ��һ�����ţ�����ɾ��������ִ�й���ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelDepartment(string id)
        {
            var target = new hrm_department() { id = id };
            var db = YSLContextFactory.Create();
            try
            {
                db.hrm_department.Attach(target);
                db.hrm_department.Remove(target);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("ɾ������ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ���ݲ���ID��ȡһ�����ŵ�����Ա��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult GetEmployeeByDepartmentId(string id)
        {
            var db = YSLContextFactory.Create();
            List<hrm_employee> result;
            try
            {
                var ids = db.hrm_department_employee.Where(m => m.department_id == id).Select(m => m.employee_id);
                var es = db.hrm_employee.Where(m => ids.Contains(m.id)).ToList();
                result = es;
            }
            catch
            {
                return ResultToJson.ToError("���ݲ���ID��ȡһ�����ŵ�����Ա��ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess(result);
        }
    }
}