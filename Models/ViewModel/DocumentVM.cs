namespace RealTimeCollaborativeApp.Models.ViewModel
{
    public class DocumentVM
    {
        public DocumentVM()
        {
            Documents = new List<Document>();
        }
        public int MaxDocsAllowed { get; set; }
        public IList<Document> Documents { get; set; }
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public bool AllowAddDocument => Documents == null || Documents.Count < MaxDocsAllowed;
    }
}
