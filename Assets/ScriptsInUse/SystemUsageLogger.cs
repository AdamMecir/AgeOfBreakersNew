using System;
using System.Diagnostics;
using System.IO;
using UnityEngine;

public class SystemUsageLogger : MonoBehaviour
{
    private string logFilePath;
    private StreamWriter logWriter;

    private Stopwatch totalCpuStopwatch;
    private TimeSpan lastTotalProcessorTime;

    private float startTime;
    private int frameCount;
    private float deltaTimeAccum;
    private float logTimer;

    [Tooltip("How often (in seconds) we want to write data to the log.")]
    [SerializeField] private float logInterval = 1f;

    private void Start()
    {
        // 1) Path to the log file
        string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string logFolder = Path.Combine(homePath, "Logs");

        if (!Directory.Exists(logFolder))
        {
            Directory.CreateDirectory(logFolder); // Create folder if it doesn't exist
        }

        logFilePath = Path.Combine(logFolder,
            $"SystemUsageLog_DONTUseObjectPool{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt");

        // 2) Open file and write header
        logWriter = new StreamWriter(logFilePath, false);
        logWriter.WriteLine("TimeSinceStart(s),FPS,CPU Usage(%),Managed Memory(MB)");
        logWriter.Flush();

        // 3) Initialize CPU measurement
        totalCpuStopwatch = new Stopwatch();
        totalCpuStopwatch.Start();
        lastTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;

        // 4) Save the start time
        startTime = Time.time;
    }

    private void Update()
    {
        // Count FPS
        frameCount++;
        deltaTimeAccum += Time.deltaTime;

        // Countdown for logging
        logTimer += Time.deltaTime;
        if (logTimer >= logInterval)
        {
            LogSystemUsage();
            logTimer = 0f;
        }
    }

    private void LogSystemUsage()
    {
        // 1) Calculate FPS over the last interval
        float avgDeltaTime = deltaTimeAccum / frameCount;
        float fps = (avgDeltaTime > 0) ? (1f / avgDeltaTime) : 0f;

        frameCount = 0;
        deltaTimeAccum = 0f;

        // 2) Calculate CPU usage since last measurement
        TimeSpan currentTotalProcessorTime = Process.GetCurrentProcess().TotalProcessorTime;
        double cpuUsedMs = (currentTotalProcessorTime - lastTotalProcessorTime).TotalMilliseconds;
        double realTimeMs = totalCpuStopwatch.Elapsed.TotalMilliseconds;

        float cpuUsage = 0f;
        if (realTimeMs > 0)
        {
            // Normalized by all cores (0-100% total usage across CPU)
            cpuUsage = (float)(cpuUsedMs / (realTimeMs * Environment.ProcessorCount) * 100f);
        }

        lastTotalProcessorTime = currentTotalProcessorTime;
        totalCpuStopwatch.Restart();

        // 3) Managed memory usage (in MB)
        long managedMemoryMB = GC.GetTotalMemory(false) / (1024 * 1024);

        // 4) Time since start (seconds), rounded to an integer
        int timeSinceStartInt = Mathf.RoundToInt(Time.time - startTime);

        // 5) Round FPS and CPU usage to integers
        int fpsInt = Mathf.RoundToInt(fps);
        int cpuUsageInt = Mathf.RoundToInt(cpuUsage);

        // 6) Write data to file
        string logLine = $"{timeSinceStartInt},{fpsInt},{cpuUsageInt},{managedMemoryMB}";
        lock (logWriter)
        {
            logWriter.WriteLine(logLine);
            logWriter.Flush();
        }
    }

    private void OnDestroy()
    {
        // Close file when the object is destroyed
        if (logWriter != null)
        {
            logWriter.Close();
            logWriter.Dispose();
        }
    }
}
