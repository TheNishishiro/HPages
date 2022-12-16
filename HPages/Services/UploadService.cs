using System;
using System.Collections.Generic;
using System.Linq;
using HPages.Database.Tables;
using HPages.Models;

namespace HPages.Services
{
    public interface IUploadService
    {
        void Reset();
        UploadResult GetCurrentUploadResult();
        void AddData(byte[] data, List<long> similar);
        bool HasRepeats();
        void SetTextImport(string text);
        bool IsTextUpload();
    }

    public class UploadService : IUploadService
    {
        private UploadResult _uploadResult;
        private bool _isTextUpload;
        
        public UploadService()
        {
            Reset();
        }

        public void SetTextImport(string text)
        {
            _isTextUpload = true;
            var lines = text.Split('\n');
            foreach (var line in lines)
            {
                var entry = line.Split(':');
                if (entry.Length <= 1 || string.IsNullOrWhiteSpace(entry[1])) continue;
                
                var indexes = entry[1].Split(',');
                _uploadResult.ImageIdsPair.Add(entry[0], indexes.Select(x => long.Parse(x.Trim())).ToList());
            }
        }
        
        public void AddData(byte[] data, List<long> similar)
        {
            if (!_uploadResult.ImageIdsPair.ContainsKey(Convert.ToBase64String(data)))
                _uploadResult.ImageIdsPair.Add(Convert.ToBase64String(data), similar);
        }

        public void Reset()
        {
            _uploadResult = new UploadResult();
            _isTextUpload = false;
        }

        public UploadResult GetCurrentUploadResult() => _uploadResult;
        public bool HasRepeats() => _uploadResult.ImageIdsPair.Any();
        public bool IsTextUpload() => _isTextUpload;
    }
}