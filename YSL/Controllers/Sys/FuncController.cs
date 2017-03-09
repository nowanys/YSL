using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YSL.Model;
using YSL.Common;
using Microsoft.EntityFrameworkCore;

namespace YSL.Controllers.Sys
{
    public class FuncController : Controller
    {
        /// <summary>
        /// ��ȡ����ϵͳȨ��
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAllFunc()
        {
            List<sys_func> data;
            var db = YSLContextFactory.Create();
            try
            {
                data = db.sys_func.ToList();
            }
            catch
            {
                return ResultToJson.ToError("��ȡ����ϵͳȨ���쳣��");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(data);
            return result;
        }
        /// <summary>
        /// �������޸�ϵͳȨ��
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveFunc(sys_func obj)
        {
            var db = YSLContextFactory.Create();
            var addFlag = false;
            if (string.IsNullOrEmpty(obj.id))
            {
                obj.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            try
            {
                db.Entry(obj).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("�������޸�ϵͳȨ��ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ɾ��ϵͳȨ�ޣ�����ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelFunc(string id)
        {
            var target = new sys_func() { id = id };
            var db = YSLContextFactory.Create();
            try
            {
                db.sys_func.Attach(target);
                db.sys_func.Remove(target);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("ɾ��ϵͳȨ��ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
    }
}