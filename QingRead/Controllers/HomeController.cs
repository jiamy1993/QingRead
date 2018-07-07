using Read.BLL;
using Read.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace QingRead.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            return View();
        }

        #region 接口
        /// <summary>
        /// 添加用户
        /// </summary>
        /// <param name="openID"></param>
        /// <param name="nickname"></param>
        /// <returns></returns>
        public JsonResult GetUserInfo(string openid, string nickname)
        {
            try
            {
                UserModel model = new UserModel();
                UserBLL bll = new UserBLL();
                model = bll.GetUserModelByID(openid);
                if (model != null)
                {
                    return Json(new { Model = model, IsExist = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model = new UserModel();
                    model.OpenID = openid;
                    model.NickName = nickname;
                    bll.AddUser(model);
                    return Json(new { Model = model, IsExist = false }, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除用户
        /// </summary>
        /// <param name="openID"></param>
        /// <returns></returns>
        public void DeleteUser(string openid)
        {
            try
            {
                UserModel model = new UserModel();
                UserBLL bll = new UserBLL();
                model.OpenID = openid;
                bll.Update(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 获取日记信息
        /// </summary>
        /// <param name="openid"></param>
        /// <returns></returns>
        public JsonResult GetDiaryInfo(string openid)
        {
            try
            {
                DiaryBLL bll = new DiaryBLL();
                DataTable dt = bll.GetDiaryToTable(openid);

                if (dt != null && dt.Rows.Count > 0)
                {
                    return Json(new { Model = TableToList(dt), IsExist = true }, JsonRequestBehavior.AllowGet);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加日记
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="nickname"></param>
        /// <param name="content"></param>
        public void AddDiary(string openid, string nickname, string content)
        {
            try
            {
                string[] arrStr = content.Replace("\"","").Replace("}","").Split(new char[2] { ',',':'});
                DiaryBLL bll = new DiaryBLL();
                DiaryModel model = new DiaryModel();
                model.OpenID = openid;
                model.NickName = nickname;
                model.Weather = arrStr[3];
                model.City = arrStr[5];
                model.DiaryContent = arrStr[7];
                bll.AddDiary(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 删除日记
        /// </summary>
        /// <param name="openid"></param>
        public void DeleteDiary(string id)
        {
            try
            {
                DiaryBLL bll = new DiaryBLL();
                DiaryModel model = new DiaryModel();
                model.ID = Guid.Parse(id);
                bll.DeleteDiary(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 修改日记
        /// </summary>
        /// <param name="openid"></param>
        public void UpdateDiary(string id,string content)
        {
            try
            {
                DiaryBLL bll = new DiaryBLL();
                DiaryModel model = new DiaryModel();
                model.ID = Guid.Parse(id);
                model.DiaryContent = content;
                bll.UpdateDiary(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 添加登录日志
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="nickname"></param>
        public void AddLoginLog(string openid, string nickname)
        {
            try
            {
                LoginLogBLL bll = new LoginLogBLL();
                LoginLogModel model = new LoginLogModel();
                model.OpenID = openid;
                model.NickName = nickname;
                bll.AddLoginLog(model);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 日记放入list
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<DiaryModel> TableToList(DataTable dt)
        {
            try
            {
                List<DiaryModel> list = new List<DiaryModel>();

                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    list.Add(new DiaryModel()
                    {
                        ID = Guid.Parse(dt.Rows[i]["ID"].ToString()),
                        OpenID = dt.Rows[i]["OpenID"].ToString(),
                        NickName = dt.Rows[i]["NickName"].ToString(),
                        DiaryContent = dt.Rows[i]["DiaryContent"].ToString(),
                        City = dt.Rows[i]["City"].ToString(),
                        Weather= dt.Rows[i]["Weather"].ToString(),
                        Createtime = DateTime.Parse(dt.Rows[i]["Createtime"].ToString()).ToString("yyyy-MM-dd HH:mm"),
                        Modifytime = DateTime.Parse(dt.Rows[i]["Modifytime"].ToString()).ToString("yyyy-MM-dd HH:mm"),
                        Disabled = int.Parse(dt.Rows[i]["Disabled"].ToString()
                                                )
                    });
                }

                return list;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JsonResult cs(string code)
        {
            return Json(new { Model = GetOpenidByCode(code)},JsonRequestBehavior.AllowGet);
        }

        public string GetOpenidByCode(string code)
        {
            string url = "https://api.weixin.qq.com/sns/oauth2/access_token?appid=wxcfe39f29a5d2cd3a&secret=23315f9071ddc1c87409001621f081c3&code=" + code + "&grant_type=authorization_code";
            string html = string.Empty;

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);
                request.Method = "GET";

                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                Stream ioStream = response.GetResponseStream();
                StreamReader sr = new StreamReader(ioStream, Encoding.Default);
                html = sr.ReadToEnd();
                sr.Close();
                ioStream.Close();
                response.Close();
                return html;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
        #endregion
}