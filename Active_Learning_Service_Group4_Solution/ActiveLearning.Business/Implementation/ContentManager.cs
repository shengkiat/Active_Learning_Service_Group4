﻿using ActiveLearning.Business.Interface;
using ActiveLearning.DB;
using ActiveLearning.Repository;
using ActiveLearning.Repository.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ActiveLearning.Common;
using System.Web;
using System.IO;
using System.Web.Mvc;
using System.Transactions;

namespace ActiveLearning.Business.Implementation
{
    public class ContentManager : BaseManager, IContentManager
    {
        public Content GetContentByContentSid(int contentSid, out string message)
        {
            message = string.Empty;
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    var content = unitOfWork.Contents.Find(c => c.Sid == contentSid && !c.DeleteDT.HasValue).FirstOrDefault();
                    if (content == null)
                    {
                        message = Constants.ValueNotFound(Constants.Content);
                        return null;
                    }
                    message = string.Empty;
                    return content;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringRetrievingValue(Constants.Content);
                return null;
            }
        }
        public Content GetAcceptedContentByContentSid(int contentSid, out string message)
        {
            var content = GetContentByContentSid(contentSid, out message);

            if (content == null)
            {
                return null;
            }

            if (content.Status.Equals(Constants.Accepted_Code, StringComparison.CurrentCultureIgnoreCase))
            {
                message = string.Empty;
                return content;
            }
            else
            {
                message = Constants.ValueNotFound(Constants.Content);
                return null;
            }

        }
        public Content GetCommentedContentByContentSid(int contentSid, out string message)
        {
            var content = GetContentByContentSid(contentSid, out message);

            if (content == null)
            {
                return null;
            }

            if (content.Status.Equals(Constants.Commented_Code, StringComparison.CurrentCultureIgnoreCase))
            {
                message = string.Empty;
                return content;
            }
            else
            {
                message = Constants.ValueNotFound(Constants.Content);
                return null;
            }
        }
        public Content GetRejectedContentByContentSid(int contentSid, out string message)
        {
            var content = GetContentByContentSid(contentSid, out message);

            if (content == null)
            {
                return null;
            }

            if (content.Status.Equals(Constants.Rejected_Code, StringComparison.CurrentCultureIgnoreCase))
            {
                message = string.Empty;
                return content;
            }
            else
            {
                message = Constants.ValueNotFound(Constants.Content);
                return null;
            }
        }
        public IEnumerable<Content> GetAllContentsByCourseSid(int courseSid, out string message)
        {
            message = string.Empty;
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    var contents = unitOfWork.Contents.Find(c => c.CourseSid == courseSid && !c.DeleteDT.HasValue);
                    if (contents == null || contents.Count() == 0)
                    {
                        message = Constants.ThereIsNoValueFound(Constants.Content);
                        return null;
                    }
                    message = string.Empty;
                    return contents.ToList();
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringRetrievingValue(Constants.Content);
                return null;
            }
        }
        public IEnumerable<Content> GetPendingContentsByCourseSid(int courseSid, out string message)
        {
            var contents = GetAllContentsByCourseSid(courseSid, out message);

            if (contents == null)
            {
                return null;
            }

            var filteredContents = contents.Where(c => c.Status.Equals(Constants.Pending_Code, StringComparison.CurrentCultureIgnoreCase));

            if (filteredContents == null || filteredContents.Count() == 0)
            {
                message = Constants.ThereIsNoValueFound(Constants.Content);
                return null;
            }
            message = string.Empty;
            return filteredContents;
        }
        public IEnumerable<Content> GetAcceptedContentsByCourseSid(int courseSid, out string message)
        {
            var contents = GetAllContentsByCourseSid(courseSid, out message);

            if (contents == null)
            {
                return null;
            }

            var filteredContents = contents.Where(c => c.Status.Equals(Constants.Accepted_Code, StringComparison.CurrentCultureIgnoreCase));

            if (filteredContents == null || filteredContents.Count() == 0)
            {
                message = Constants.ThereIsNoValueFound(Constants.Content);
                return null;
            }
            message = string.Empty;
            return filteredContents;
        }
        public IEnumerable<Content> GetCommentedContentsByCourseSid(int courseSid, out string message)
        {
            var contents = GetAllContentsByCourseSid(courseSid, out message);

            if (contents == null)
            {
                return null;
            }

            var filteredContents = contents.Where(c => c.Status.Equals(Constants.Commented_Code, StringComparison.CurrentCultureIgnoreCase));

            if (filteredContents == null || filteredContents.Count() == 0)
            {
                message = Constants.ThereIsNoValueFound(Constants.Content);
                return null;
            }
            message = string.Empty;
            return filteredContents;
        }
        public IEnumerable<Content> GetRejectedContentsByCourseSid(int courseSid, out string message)
        {
            var contents = GetAllContentsByCourseSid(courseSid, out message);

            if (contents == null)
            {
                return null;
            }

            var filteredContents = contents.Where(c => c.Status.Equals(Constants.Rejected_Code, StringComparison.CurrentCultureIgnoreCase));

            if (filteredContents == null || filteredContents.Count() == 0)
            {
                message = Constants.ThereIsNoValueFound(Constants.Content);
                return null;
            }
            message = string.Empty;
            return filteredContents;
        }
        public IEnumerable<Content> GetAllPendingContents(out string message)
        {
            var contentList = new List<Content>();
            message = string.Empty;
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    var contents = unitOfWork.Contents.Find(c => !c.DeleteDT.HasValue && c.Status.Equals(Constants.Pending_Code, StringComparison.CurrentCultureIgnoreCase));
                    if (contents == null || contents.Count() == 0)
                    {
                        message = Constants.ThereIsNoValueFound("pending content");
                        return null;
                    }

                    foreach (var content in contents)
                    {
                        content.Course = unitOfWork.Courses.Find(c => c.Sid == content.CourseSid && !c.DeleteDT.HasValue).FirstOrDefault();
                        if (content.Course != null)
                        {
                            contentList.Add(content);
                        }
                    }

                    message = string.Empty;
                    return contentList;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringRetrievingValue(Constants.Content);
                return null;
            }
        }
        public IEnumerable<int> GetAllContentSidsByCounrseSid(int courseSid, out string message)
        {
            var contents = GetAllContentsByCourseSid(courseSid, out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            return contents.Select(c => c.Sid).ToList();
        }
        public IEnumerable<int> GetPendingContentSidsByCounrseSid(int courseSid, out string message)
        {
            var contents = GetPendingContentsByCourseSid(courseSid, out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            return contents.Select(c => c.Sid).ToList();
        }
        public IEnumerable<int> GetAcceptedContentSidsByCounrseSid(int courseSid, out string message)
        {
            var contents = GetAcceptedContentsByCourseSid(courseSid, out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            return contents.Select(c => c.Sid).ToList();
        }
        public IEnumerable<int> GetCommentedContentSidsByCounrseSid(int courseSid, out string message)
        {
            var contents = GetCommentedContentsByCourseSid(courseSid, out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            return contents.Select(c => c.Sid).ToList();
        }
        public IEnumerable<int> GetRejectedContentSidsByCounrseSid(int courseSid, out string message)
        {
            var contents = GetRejectedContentsByCourseSid(courseSid, out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            message = string.Empty;
            return contents.Select(c => c.Sid).ToList();
        }
        public IEnumerable<int> GetAllPendingContentSids(out string message)
        {
            var contents = GetAllPendingContents(out message);
            if (contents == null || contents.Count() == 0)
            {
                return null;
            }
            message = string.Empty;
            return contents.Select(c => c.Sid).ToList();
        }
        public string GetContentPathByContentSid(int contentSid, out string message)
        {
            message = string.Empty;
            string path = string.Empty;
            try
            {
                var content = GetContentByContentSid(contentSid, out message);
                if (content == null)
                {
                    message = Constants.ValueNotFound(Constants.Content);
                    return null;
                }
                path = content.Path + content.FileName;
                return path;
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringRetrievingValue(Constants.File);
                return null;
            }


            //message = string.Empty;
            //using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
            //{
            //    var file = unitOfWork.Contents.GetAll().Where(c => c.CourseSid == courseID && c.DeleteDT == null && c.OriginalFileName == originalFilename).FirstOrDefault();

            //    return file.FileName;
            //}
        }
        public Content AddContent(string physicalUploadPath, HttpPostedFileBase file, int courseSid, out string message)
        {
            message = string.Empty;

            if (file == null || file.ContentLength == 0 || String.IsNullOrEmpty(file.FileName))
            {
                message = Constants.ValueIsEmpty(Constants.File);
                return null;
            }
            if (courseSid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.Course);
                return null;
            }

            var fileName = file.FileName;
            var fileExtension = Path.GetExtension(file.FileName);
            var fileSize = file.ContentLength;

            if (String.IsNullOrEmpty(fileExtension))
            {
                message = Constants.UnknownValue(Constants.FileExtension);
                return null;
            }

            var allowedFileExtension = Util.GetAllowedFileExtensionFromConfig();


            if (!allowedFileExtension.Contains(fileExtension))
            {
                message = Constants.OnlyValueAllowed(allowedFileExtension.Replace(".", ""));
                return null;
            }

            var allowedFileSize = Util.GetAllowedFileSizeFromConfig();

            if (fileSize > allowedFileSize * 1024 * 1024)
            {
                message = Constants.ValueNotAllowed(Constants.FileSize);
                return null;
            }

            string GUIDFileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var uploadFolder = Util.GetUploadFolderFromConfig();

            try
            {
                var uploadPath = Path.Combine(physicalUploadPath, GUIDFileName);
                file.SaveAs(uploadPath);
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringSavingValue(Constants.File);
                return null;
            }

            try
            {
                Content content = new Content();
                content.CourseSid = courseSid;
                content.CreateDT = DateTime.Now;
                content.FileName = GUIDFileName;
                content.OriginalFileName = file.FileName;
                content.Status = Constants.Pending_Code;
                if (Util.GetVideoFormatsFromConfig().Contains(fileExtension))
                {
                    content.Type = Constants.Content_Type_Video;
                }
                else
                {
                    content.Type = Constants.Content_Type_File;
                }
                content.Path = uploadFolder;

                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    unitOfWork.Contents.Add(content);
                    unitOfWork.Complete();
                    return content;
                }
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringAddingValue(Constants.File);
                return null;
            }
        }
        public bool DeleteContent(string physicalFilePath, int contentSid, out string message)
        {
            message = string.Empty;
            if (string.IsNullOrEmpty(physicalFilePath))
            {
                message = Constants.ValueIsEmpty(Constants.FilePath);
                return false;
            }
            if (contentSid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.File);
                return false;
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    var content = unitOfWork.Contents.Get(contentSid);
                    File.Delete(physicalFilePath);
                    content.DeleteDT = DateTime.Now;
                    unitOfWork.Complete();
                }
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringDeletingValue(Constants.File);
                return false;
            }
        }
        public bool UpdateContent(Content content, out string message)
        {
            message = string.Empty;
            if (content == null || content.Sid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.Content);
                return false;
            }
            var contentToUpdate = GetContentByContentSid(content.Sid, out message);
            if (contentToUpdate == null)
            {
                return false;
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    Util.CopyNonNullProperty(content, contentToUpdate);
                    contentToUpdate.UpdateDT = DateTime.Now;
                    using (TransactionScope scope = new TransactionScope())
                    {
                        unitOfWork.Complete();
                        scope.Complete();
                    }
                }
                message = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringUpdatingValue(Constants.Content);
                return false;
            }
        }
        public bool UpdateContentStatus(int contentSid, string status, string remark, out string message)
        {
            message = string.Empty;
            if (contentSid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.Content);
                return false;
            }
            if (string.IsNullOrEmpty(status))
            {
                message = Constants.ValueIsEmpty(Constants.Status);
                return false;
            }
            switch (status)
            {
                case Constants.Pending_Code:
                case Constants.Accepted_Code:
                case Constants.Rejected_Code:
                    remark = string.Empty;
                    break;
                case Constants.Commented_Code:
                    if (string.IsNullOrEmpty(remark))
                    {
                        message = Constants.ValueIsEmpty(Constants.Remark);
                        return false;
                    }
                    break;
                default:
                    message = Constants.UnknownValue(Constants.Status);
                    return false;
            }
            var content = GetContentByContentSid(contentSid, out message);
            if (content == null)
            {
                return false;
            }
            try
            {
                using (var unitOfWork = new UnitOfWork(new ActiveLearningContext()))
                {
                    var contentToUpdate = unitOfWork.Contents.Get(content.Sid);
                    contentToUpdate.UpdateDT = DateTime.Now;
                    contentToUpdate.Status = status;
                    contentToUpdate.Remark = remark;
                    using (TransactionScope scope = new TransactionScope())
                    {
                        unitOfWork.Complete();
                        scope.Complete();
                    }
                }
                message = string.Empty;
                return true;
            }
            catch (Exception ex)
            {
                ExceptionLog(ex);
                message = Constants.OperationFailedDuringUpdatingValue(Constants.Content);
                return false;
            }
        }
        public bool AcceptContent(Content content, out string message)
        {
            message = string.Empty;
            if (content == null || content.Sid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.Content);
                return false;
            }
            return UpdateContentStatus(content.Sid, Constants.Accepted_Code, content.Remark, out message);
        }
        public bool CommentContent(Content content, out string message)
        {
            message = string.Empty;
            if (content == null || content.Sid == 0)
            {
                message = Constants.ValueIsEmpty(Constants.Content);
                return false;
            }
            return UpdateContentStatus(content.Sid, Constants.Commented_Code, content.Remark, out message);
        }
      }
}
