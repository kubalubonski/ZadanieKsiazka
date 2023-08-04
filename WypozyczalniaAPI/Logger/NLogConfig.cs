using NLog.Config;
using NLog;
using NLog.Layouts;

namespace WypozyczalniaAPI;

public static class NLogConfig
{
    public static LoggingConfiguration ConfigureNLog()
    {
        var config = new LoggingConfiguration();

        //Target
        var fileTarget1 = new NLog.Targets.FileTarget("AllLogers");
        
        fileTarget1.FileName = "C:\\Users\\Jakub Luboński\\Desktop\\ZadanieKsiazka\\WypozyczalniaApiLogs\nlog-all-${shortdate}.log";
        fileTarget1.Layout = //"${longdate}|${event-properties:item=EventId_Id}|${uppercase:${level}}|${logger}|${message} ${exception:format=tostring}";
        new JsonLayout
                {
                    Attributes =
                    {
                        new JsonAttribute("time", "${longdate}"),
                        new JsonAttribute("level", "${level:uppercase=true}"),
                        new JsonAttribute("logger", "${logger}"),
                        new JsonAttribute("message", "${message}"),
                        new JsonAttribute("exception", "${exception:format=tostring}"),
                        new JsonAttribute("request_id", "${event-properties:item=X-Request-ID}")
                    }
                };
         config.AddTarget(fileTarget1);
        //Rules
        var rule1 = new LoggingRule("WypozyczalniaAPI*", NLog.LogLevel.Info, fileTarget1);
        NLog.LogManager.Configuration = config;
        return config;

    }
}