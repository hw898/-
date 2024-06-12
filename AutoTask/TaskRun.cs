﻿using MobileControlGuru.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using MobileControlGuru.Tools;
using MobileControlGuru.Src;
using static MobileControlGuru.AutoTask.TaskRun;
namespace MobileControlGuru.AutoTask
{
    public class TaskRun
    {
        TaskJson.TaskInfo taskInfo;
        string DeviceName;
        bool Debug=false;
        
        public delegate void TaskStartDelegate();
        public delegate void TaskFinishedDelegate();
        
        public delegate void TaskSingleEndDelegate(string cmd);
        public delegate void TaskSingleStartDelegate(string cmd);

        //任务开始委托
        public TaskStartDelegate taskStartDelegate;
        //任务结束委托
        public TaskFinishedDelegate taskFinishedDelegate;

        //单条指令开始
        public TaskSingleStartDelegate singleStartDelegate;
        //单条指令结束 
        public TaskSingleEndDelegate singleEndDelegate;
        public CancellationTokenSource cts = new CancellationTokenSource();
        public TaskRun(string deviceName, TaskJson.TaskInfo taskInfo, bool debug=false)
        {
            DeviceName = deviceName;
            this.taskInfo = taskInfo;
            Debug = debug;
        }

        public string R(TaskJson.TaskItem taskItem)
        {
            try
            {
               
                singleStartDelegate(taskItem.Oprate + " " + taskItem.Param);
                string res = "";
                if (taskItem.IsAdb)
                {
                  
                    var adb= ADB.Exec("-s "+ DeviceName + " "+taskItem.Oprate + " " + taskItem.Param); 
                    res= adb.Message;
                }
                else
                {
                    if (taskItem.Oprate == "sleep")
                    {
                        Thread.Sleep(Convert.ToInt32(taskItem.Param));
                    }
                    else
                    {
                        res=  CMD.Exec(taskItem.Param);
                        
                    } 
                }
                singleEndDelegate(res);
                return res;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex, taskItem.Oprate+""+ taskItem.Param);
                singleEndDelegate("run error"+ ex.Message);
                return ex.Message;
            }
           

        }

        public  void Run()
        {

            if (taskStartDelegate != null)
            {
                taskStartDelegate();
            }
            if (Debug)
            {
                foreach (var t in taskInfo.TaskItems)
                {
                    if (cts.Token.IsCancellationRequested)
                    {
                        break;
                    }
                    R(t);
                }
            }
            else
            {
                int i = taskInfo.RunTimes;
                while (i != 0)
                {
                    foreach (var t in taskInfo.TaskItems)
                    {
                        if (cts.Token.IsCancellationRequested)
                        {
                            break;
                        }
                        R(t);
                    }
                    i--;
                }

            }


            if (taskFinishedDelegate != null)
            {
                taskFinishedDelegate();
            }
        }

        public  async Task RunAsync()
        {
            //using (HttpClient client = new HttpClient())
            //{
            //    string website = "https://www.example.com";
            //    string content = await client.GetStringAsync(website);
            //    Console.WriteLine("下载内容长度：" + content.Length);
            //}
        } 
    }
}
