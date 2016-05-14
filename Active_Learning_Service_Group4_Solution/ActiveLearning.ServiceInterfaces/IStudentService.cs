﻿using ActiveLearning.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ActiveLearning.ServiceInterfaces
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IService1" in both code and config file together.
    [ServiceContract]
    public interface IStudentService
    {
        [OperationContract]
        IEnumerable<Course> GetCoursesByStudentSid(int studentSid);

        [OperationContract]
        IEnumerable<Content> GetContentsByCourseSid(int courseSid);

        [OperationContract]
        QuizQuestion GetNextQuiz();

        [OperationContract]
        bool AnswerQuiz(int quizQuestionSid, int quizAnswserSid);

    }

}
