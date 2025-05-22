using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

using Azure.Data.Tables;
using Azure.Storage.Blobs;
using Azure.Core;
using Azure.Storage.Common;
using Azure;

namespace WCFServiceWebRole
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class UserEntity : ITableEntity
    {
        public UserEntity(string partitionKey, string rowKey)
        {
            PartitionKey = partitionKey;
            RowKey = rowKey;
        }
        public UserEntity() { }
        public string PartitionKey { get; set; }
        public string RowKey { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public Guid SessionId { get; set; }
        public DateTimeOffset? Timestamp { get; set; }
        public ETag ETag { get; set; }
    }

    public class Service : IService
	{
        private const string ConnectionString = "UseDevelopmentStorage=true";
        private const string TableName = "users";
        private const string PartitionKey = "users";
        private const string BlobContainerName = "userfiles";

        public bool Create(string login, string password)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (currentUser.HasValue)
            {
                return false;
            }

            var user = new UserEntity(PartitionKey, login);
            user.Login = login;
            user.Password = password;
            user.SessionId = Guid.Empty;
            
            client.AddEntity(user);
            return true;
        }

        public string Get(string name, Guid sessionId)
        {
            var userClient = GetTableClient();
            var filter = $"SessionId eq guid'{sessionId}'";
            var sessionUser = userClient.Query<UserEntity>(filter).FirstOrDefault();
            if (sessionUser == null)
            {
                return string.Empty;
            }

            var blobName = $"{sessionUser.Login}/{name}";
            var blobClient = GetBlobClient(blobName);
            if (!blobClient.Exists())
            {
                return string.Empty;
            }

            var stream = new System.IO.MemoryStream();
            try
            {
                blobClient.DownloadTo(stream);
                stream.Position = 0;
                var reader = new System.IO.StreamReader(stream);
                try
                {
                    return reader.ReadToEnd();
                }
                finally
                {
                    reader.Dispose();
                }
            }
            finally
            {
                stream.Dispose();
            }
        }

        public Guid Login(string login, string password)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (!currentUser.HasValue || currentUser.Value.Password != password)
            {
                return Guid.Empty;
            }

            var newSessionId = Guid.NewGuid();
            currentUser.Value.SessionId = newSessionId;
            client.UpdateEntity(currentUser.Value, currentUser.Value.ETag);
            return newSessionId;
        }

        public bool Logout(string login)
        {
            var client = GetTableClient();

            var currentUser = client.GetEntityIfExists<UserEntity>(PartitionKey, login);

            if (!currentUser.HasValue)
            {
                return false;
            }

            currentUser.Value.SessionId = Guid.Empty;
            client.UpdateEntity(currentUser.Value, currentUser.Value.ETag);

            return true;
        }

        public bool Put(string name, string content, Guid sessionId)
        {
            var tableClient = GetTableClient();

            var filter = $"SessionId eq guid'{sessionId}'";
            var sessionUser = tableClient.Query<UserEntity>(filter).FirstOrDefault();

            if (sessionUser == null)
            {
                return false;
            }

            var blobName = $"{sessionUser.Login}/{name}";
            var blobClient = GetBlobClient(blobName);

            var stream = new System.IO.MemoryStream(Encoding.UTF8.GetBytes(content));
            try
            {
                blobClient.Upload(stream, true);
            }
            finally
            {
                stream.Dispose();
            }
            return true;
        }

        private static TableClient GetTableClient(string tableName = TableName)
        {
            var client = new TableClient(ConnectionString, tableName);
            client.CreateIfNotExists();
            return client;
        }

        private static BlobClient GetBlobClient(string blobName, string containerName = BlobContainerName)
        {
            var blobServiceClient = new BlobServiceClient(ConnectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);
            containerClient.CreateIfNotExists();
            return containerClient.GetBlobClient(blobName);
        }
    }
}
