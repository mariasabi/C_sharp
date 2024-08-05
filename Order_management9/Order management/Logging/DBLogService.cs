﻿using log4net;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Order_management.Models;
using Order_management.Service;
using System.Data;

namespace Order_management.Logging
{
    public class DBLogService
    {
        private readonly OrderManagementContext _context;
        private static readonly ILog log = LogManager.GetLogger(typeof(DBLogService));
        private readonly IHttpContextAccessor _httpContextAccessor;
        private string _userId;
        
        public DBLogService(OrderManagementContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _userId = _httpContextAccessor.HttpContext.Session.GetString("UserId");
            //ThreadContext.Properties["UserId"] = Int32.Parse(_userId);
        }
        public void CallStoredProcedure(DateTime date, string thread, string level, string logger,string message)
        {
            //var parameter = new SqlParameter("@ParameterName", 1);
            try
            { 
            var dateParameter = new SqlParameter("@Date", SqlDbType.DateTime) { Value = date };
            var threadParameter = new SqlParameter("@Thread", SqlDbType.NVarChar, 255) { Value = thread };
            var levelParameter = new SqlParameter("@Level", SqlDbType.NVarChar, 50) { Value = level };
            var loggerParameter = new SqlParameter("@Logger", SqlDbType.NVarChar, 255) { Value = logger};
            var messageParameter = new SqlParameter("@Message", SqlDbType.NVarChar, -1) { Value = message };
            var userIdParameter = new SqlParameter("@UserId", SqlDbType.Int) { Value = _userId };
            _context.Database.ExecuteSqlRaw("EXEC LogError @Dat, @Thread, @Level, @Logger, @Message, @UserId", dateParameter, threadParameter, levelParameter, loggerParameter, messageParameter, userIdParameter);
            }
            catch
            {
                log.Debug("SQL exception while adding logs to database has occurred.");
            }
        }
    }
}