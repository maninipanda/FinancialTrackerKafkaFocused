using FinancialTracker.Application.Interfaces.Persistence;
using FinancialTracker.Domain.Entities;
using FinancialTracker.Infrastructure.Persistence.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FinancialTracker.Infrastructure.Persistence.Repositories
{
    public class ProcessedMessageRepository : IProcessedMessageRepository
    {

        private readonly MyDbContext _dbContext;

        public ProcessedMessageRepository(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

      
    

        //public async Task AddAsync(ProcessedMessageM message)
        //{
        //    var entity = new ProcessedMessage
        //    {
        //        MessageKey = message.MessageKey,
        //        Offset = message.Offset,
        //        Topic = message.Topic,
        //        ProcessedAt = DateTime.UtcNow
        //    };
        //    _dbContext.ProcessedMessages.Add(entity);
        //    await _dbContext.SaveChangesAsync();
        //}

        /// <summary>
        /// Tries to insert the message into the ProcessedMessages table.
        /// Returns true if the message was inserted (first time processing).
        /// Returns false if message already exists (duplicate).
        /// </summary>

        public async Task<bool> TryAddAsync(ProcessedMessageM message)
        {
            var entity = new ProcessedMessage
            {

                MessageKey = message.MessageKey, Offset = message.Offset, Topic = message.Topic, ProcessedAt = message.ProcessedAt

            };
            try
            {
                _dbContext.ProcessedMessages.Add(entity);
                await _dbContext.SaveChangesAsync(); //Atomic insert
                return true;// first time processing


            }

            catch (DbUpdateException ex) when (IsDuplicateKeyException(ex))

            {


                // Duplicate message -> already processed
                return false;


            }
        }
            /// <summary>
            /// Detects if the exception is due to a unique constraint violation on MessageKey.
            /// </summary>
         private bool IsDuplicateKeyException(DbUpdateException ex)
        {
            // SQL Server error message contains the unique constraint name
            return ex.InnerException?.Message.Contains("UQ_MessageKey") ?? false;
        }

    }
    }
