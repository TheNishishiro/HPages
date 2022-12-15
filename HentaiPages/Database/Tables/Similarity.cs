namespace HentaiPages.Database.Tables
{
    public class Similarity
    {
        public int SimilarityId { get; set; }
        public int ParentImageId { get; set; }
        public int ChildImageId { get; set; }
        public float SimilarityScore { get; set; }
    }
}