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
    public class RoleController : Controller
    {
        /// <summary>
        /// ��ȡϵͳ���н�ɫ
        /// </summary>
        /// <returns></returns>
        public JsonResult GetAllRole()
        {
            List<sys_role> roles;
            var db = new YSLContext();
            try
            {
                roles = db.sys_role.OrderBy(m=>m.order_num).ToList();
            }
            catch
            {
                return ResultToJson.ToError("��ȡϵͳ���н�ɫ����");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(roles);
            return result;
        }
        /// <summary>
        /// Ϊһ����ɫ����һ��Ȩ��
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="funcId"></param>
        /// <returns></returns>
        public JsonResult AddRoleFunc([FromBody]sys_role_func obj)
        {
            obj.id = Guid.NewGuid().ToString("N");
            var db = new YSLContext();
            try
            {
                db.sys_role_func.Add(obj);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("Ϊһ����ɫ����һ��Ȩ��ʧ��");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ��ȡһ����ɫ������Ȩ��
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult GetRoleFunc([FromBody]sys_role obj)
        {
            var db = new YSLContext();
            List<sys_func> roles;
            try
            {
                var linq = from v in db.sys_func
                           join r in db.sys_role_func on v.id equals r.func_id
                           where r.role_id == obj.id
                           select v;
                roles = linq.ToList();
            }
            catch
            {
                return ResultToJson.ToError("��ȡһ���˻���ӵ�еĽ�ɫʧ��");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess(roles);
        }
        /// <summary>
        /// Ϊһ����ɫɾ��һ��Ȩ�ޣ�����ɾ��
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="funcId"></param>
        /// <returns></returns>
        public JsonResult DelRoleFunc([FromBody]sys_role_func obj)
        {
            var db = new YSLContext();
            try
            {
                db.sys_role_func.Attach(obj);
                db.sys_role_func.Remove(obj);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("Ϊһ����ɫɾ��һ��Ȩ��ʧ��");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// �������޸Ľ�ɫ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveRole([FromBody]sys_role role)
        {
            var db = new YSLContext();
            var addFlag = false;
            if (string.IsNullOrEmpty(role.id))
            {
                role.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            try
            {
                db.Entry(role).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("�������޸Ľ�ɫʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ����ϵͳ���Ƿ��Ѵ�����ͬ���ƵĽ�ɫ
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public JsonResult CheckRoleName([FromBody] sys_role target)
        {
            var result = true;
            var db = new YSLContext();
            try
            {
                var count = db.sys_role.Where(m => m.role_name == target.role_name).Count();
                result = count > 0;
            }
            catch
            {
                return ResultToJson.ToError("����ϵͳ���Ƿ��Ѵ�����ͬ�Ľ�ɫʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess(result);
        }
        /// <summary>
        /// ɾ����ɫ������ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelRole([FromBody] sys_role target)
        {
            var db = new YSLContext();
            try
            {
                db.sys_role.Attach(target);
                db.sys_role.Remove(target);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("ɾ����ɫʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
    }
}