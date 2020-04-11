using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UsersTasks
{
    public class MyConfiguration
    {
        public Database database { get; set; }
        public Token token { get; set; }
        public Smtp smtp { get; set; }
        public Email email { get; set; }
    }

    public class Database
    {
        public string connectionString { get; set; }
    }

    public class Token
    {
        public string issuer { get; set; }
        public string audience { get; set; }
        public int expirationHours { get; set; }
    }

    public class Smtp
    {
        public string host { get; set; }
        public int port { get; set; }
        public bool enableSsl { get; set; }
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Email
    {
        public string fromName { get; set; }
        public string fromAddress { get; set; }
        public string messageBodyPreprefix { get; set; }
        public string subjectYourTasks { get; set; }
        public string messageYourTasksBodyPrefix { get; set; }
        public string subjectSharedTasks { get; set; }
        public string messageSharedTasksBodyPrefix { get; set; }
        public string messageSuffix { get; set; }
    }
}

