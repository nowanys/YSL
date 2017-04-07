using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using YSL.Model;
using Microsoft.EntityFrameworkCore;
using YSL.Common;
using Microsoft.Extensions.Caching.Memory;

namespace YSL.Controllers.Hrm
{
    /// <summary>
    /// ���ſ�����
    /// </summary>
    public class DepartmentController : Controller
    {
        private IMemoryCache cache;
        public DepartmentController(IMemoryCache memoryCache)
        {
            this.cache = memoryCache;
        }
        /// <summary>
        /// ���ɲ��ŵĲ㼶�ṹ�ĵݹ麯��
        /// </summary>
        /// <param name="curNode"></param>
        /// <param name="allNode"></param>
        private void InitSubNode(hrm_department curNode, List<hrm_department> allNode)
        {
            foreach (var n in allNode)
            {
                if (n.pid == curNode.id)
                {
                    if (curNode.children == null)
                    {
                        curNode.children = new List<hrm_department>();
                    }
                    curNode.children.Add(n);
                    InitSubNode(n, allNode);//todo:����д�ݹ飬����һ���ӣ�
                }
            }
        }
        /// <summary>
        /// ��ȡ����ϵͳȨ��
        /// </summary>
        /// <returns></returns>
        public IActionResult GetAllDepartmentTree()
        {
            List<hrm_department> tree;
            if (this.cache.TryGetValue<List<hrm_department>>("Func_Tree", out tree))
            {
                return ResultToJson.ToSuccess(tree);
            }
            List<hrm_department> data;
            var db = new YSLContext();
            try
            {
                data = db.hrm_department.OrderBy(m => m.order_num).ToList();
            }
            catch
            {
                return ResultToJson.ToError("��ȡ����ϵͳȨ���쳣��");
            }
            finally
            {
                db.Dispose();
            }
            tree = new List<hrm_department>();
            foreach (var item in data)
            {
                if (string.IsNullOrEmpty(item.pid))
                {
                    tree.Add(item);
                    InitSubNode(item, data);
                }
            }
            cache.Set("Func_Tree", tree);
            var result = ResultToJson.ToSuccess(tree);
            return result;
        }
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
            var db = new YSLContext();
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
            var db = new YSLContext();
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
            var db = new YSLContext();
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