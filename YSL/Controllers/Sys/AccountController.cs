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
    public class AccountController : Controller
    {
        /// <summary>
        /// ��ȡ�ʺ���Ϣ
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetAccountByPage([FromBody]PageDataRequestModel page)
        {
            List<sys_account> data;
            int rowCount = 0;
            var db = new YSLContext();
            try
            {
                var query = db.sys_account
                    .OrderByDescending(m => m.add_time)
                    .Skip(page.page_index * page.page_size)
                    .Take(page.page_size)
                    .Select(m => new sys_account
                    {
                        id = m.id,
                        account_name = m.account_name,
                        add_time = m.add_time,
                        employee_id = m.employee_id
                    });
                data = query.ToList();
                rowCount = db.sys_account.Count();
            }
            catch
            {
                return ResultToJson.ToError("��ȡ����ϵͳȨ���쳣��");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(rowCount,data);
            return result;
        }
        /// <summary>
        /// ϵͳ��¼
        /// </summary>
        /// <returns></returns>
        public JsonResult Login(sys_account account)
        {
            account.pass_word = Encrypt.getMD5Code(account.pass_word);
            sys_account cur_user;
            List<string> funcIds;
            var db = new YSLContext();
            try
            {

                cur_user = db.sys_account.Where(m => m.account_name == account.account_name && m.pass_word == account.pass_word).First();
                var roleIds = db.sys_account_role.Where(m => m.account_id == cur_user.id).Select(m => m.role_id);
                funcIds = db.sys_role_func.Where(m => roleIds.Contains(m.role_id)).Select(m => m.func_id).ToList();
            }
            catch
            {
                return ResultToJson.ToError("�û������������");
            }
            finally
            {
                db.Dispose();
            }
            HttpContext.Session.Set("CurUser", cur_user);
            HttpContext.Session.Set("CurUserFunc", funcIds);
            var obj = new { FuncIds = funcIds, Account = cur_user };
            return ResultToJson.ToSuccess(obj);
        }
        /// <summary>
        /// �������޸��˻�
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveAccount([FromBody]sys_account account)
        {
            account.pass_word = Encrypt.getMD5Code(account.pass_word);
            var addFlag = false;
            if (string.IsNullOrEmpty(account.id))
            {
                account.id = Guid.NewGuid().ToString("N");
                account.add_time = DateTime.Now;
                addFlag = true;
            }
            var db = new YSLContext();
            try
            {
                db.Entry(account).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("�������޸��˻�ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ����ϵͳ���Ƿ��Ѵ�����ͬ���˻�
        /// </summary>
        /// <param name="target"></param>
        /// <returns></returns>
        public JsonResult CheckAccountName([FromBody] sys_account target)
        {
            var result = true;
            var db = new YSLContext();
            try
            {
                var count = db.sys_account.Where(m => m.account_name == target.account_name).Count();
                result = count > 0;
            }
            catch
            {
                return ResultToJson.ToError("����ϵͳ���Ƿ��Ѵ�����ͬ���˻�ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess(result);
        }
        /// <summary>
        /// ɾ���˻�������ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DelAccount([FromBody]sys_account target)
        {
            var db = new YSLContext();
            try
            {
                db.sys_account.Attach(target);
                db.sys_account.Remove(target);
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
        /// Ϊһ���˻�����һ����ɫ
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="funcId"></param>
        /// <returns></returns>
        public JsonResult AddAccountRole(sys_account_role obj)
        {
            obj.id = Guid.NewGuid().ToString("N");
            var db = new YSLContext();
            try
            {
                db.sys_account_role.Add(obj);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("Ϊһ���˻�����һ����ɫʧ��");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// Ϊһ���˻�ɾ��һ����ɫ������ɾ��
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="funcId"></param>
        /// <returns></returns>
        public JsonResult DelRoleFunc(sys_account_role obj)
        {
            var db = new YSLContext();
            try
            {
                db.sys_account_role.Attach(obj);
                db.sys_account_role.Remove(obj);
                db.SaveChanges();
            }
            catch
            {
                return ResultToJson.ToError("Ϊһ���˻�ɾ��һ����ɫʧ��");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
    }
}