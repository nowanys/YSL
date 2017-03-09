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
            var db = YSLContextFactory.Create();
            try
            {
                roles = db.sys_role.ToList();
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
        public JsonResult AddRoleFunc(sys_role_func obj)
        {
            obj.id = Guid.NewGuid().ToString("N");
            var db = YSLContextFactory.Create();
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
        /// Ϊһ����ɫɾ��һ��Ȩ�ޣ�����ɾ��
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="funcId"></param>
        /// <returns></returns>
        public JsonResult DelRoleFunc(sys_role_func obj)
        {
            var db = YSLContextFactory.Create();
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
        public JsonResult SaveAccount(sys_role role)
        {
            var db = YSLContextFactory.Create();
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
        /// ɾ����ɫ������ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelAccount(string id)
        {
            var target = new sys_account() { id = id };
            var db = YSLContextFactory.Create();
            try
            {
                db.sys_account.Attach(target);
                db.sys_account.Remove(target);
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