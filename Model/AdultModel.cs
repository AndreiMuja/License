using System;

namespace PrevenireRiscIT.Model
{
    [Serializable]
    public class AdultModel
    {
        public Adult adult { get; set; }
        public Metadata metadata { get; set; }
        public string requestId { get; set; }
    }
    [Serializable]
    public class Adult
    {
        public double adultScore { get; set; }
        public double racyScore { get; set; }
        public bool isAdultContent { get; set; }
        public bool isRacyContent { get; set; }
    }
    [Serializable]
    public class Metadata
    {
        public string format { get; set; }
        public int height { get; set; }
        public int width { get; set; }
    }
}