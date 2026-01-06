namespace PPProject.Resource.Model
{
    public class UserResource
    {
        public long uId { get; set; }
        public int resourceId { get; set; }
        public int amount { get; set; }
        public DateTime createdTime { get; set; }
        public DateTime updateTime { get; set;}
    }
}
