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
    /// �γ̿�����
    /// </summary>
    public class CourseController : Controller
    {
        /// <summary>
        /// ��ȡ�γ��б�
        /// </summary>
        /// <param name="page"></param>
        /// <returns></returns>
        public JsonResult GetCourseByPage([FromBody]JObject form)
        {
            var page = form["pager"].ToObject<PageDataRequestModel>();
            var searchTxt = form["searchTxt"] == null ? "" : form["searchTxt"].ToString();
            List<hrm_course> data;
            int rowCount = 0;
            var db = new YSLContext();
            try
            {
                var query = db.hrm_course
                    .Where(m => m.course_name.Contains(searchTxt))
                    .OrderByDescending(m => m.level);
                rowCount = query.Count();
                data = query.Skip(page.page_index * page.page_size)
                    .Take(page.page_size).ToList();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("��ȡ�γ��б��쳣��");
            }
            finally
            {
                db.Dispose();
            }
            var result = ResultToJson.ToSuccess(rowCount, data);
            return result;
        }
        /// <summary>
        /// �������޸Ŀγ���Ŀ
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public IActionResult SaveCourse([FromBody]hrm_course course)
        {
            var addFlag = false;
            if (string.IsNullOrEmpty(course.id))
            {
                course.id = Guid.NewGuid().ToString("N");
                addFlag = true;
            }
            var db = new YSLContext();
            try
            {
                db.Entry(course).State = addFlag ? EntityState.Added : EntityState.Modified;
                db.SaveChanges();
            }
            catch(Exception ex)
            {
                return ResultToJson.ToError("�������޸Ŀγ���Ŀʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
        /// <summary>
        /// ɾ��һ���γ̣�����ɾ��
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DelCourse([FromBody]hrm_course target)
        {
            var db = new YSLContext();
            try
            {
                db.hrm_course.Attach(target);
                db.hrm_course.Remove(target);
                db.SaveChanges();
            }
            catch (Exception ex)
            {
                return ResultToJson.ToError("ɾ���γ�ʧ�ܣ�");
            }
            finally
            {
                db.Dispose();
            }
            return ResultToJson.ToSuccess();
        }
    }
}