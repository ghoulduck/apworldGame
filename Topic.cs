namespace AP_World_Project;

public class Topic {
    
    private readonly string topic;
    private readonly string pirates;

    public Topic(string topic, string pirates) {
        this.topic = topic;
        this.pirates = pirates;
    }
    
    public string GetTopic() {
        return topic;
    }

    public string GetPirates() {
        return pirates;
    }
}