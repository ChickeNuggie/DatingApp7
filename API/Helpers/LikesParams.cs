namespace API.Helpers
{
    public class LikesParams : PaginationParams
    {
        //Specify properties that are interested in addition to the pagination params
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }
}