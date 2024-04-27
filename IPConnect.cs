﻿using AntdUI;
using MobileControlGuru.Base;
using MobileControlGuru.Src;
using MobileControlGuru.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobileControlGuru
{
    public partial class IPConnect : BaseForm
    {
        ComponentResourceManager resources = new ComponentResourceManager(typeof(IPConnect));
        public MobileControlGuru.MainForm main { set; get; }
        public IPConnect(MobileControlGuru.MainForm _main)
        {
            this.main = _main;
            InitializeComponent();
        }



        private void connect_btn_Click(object sender, EventArgs e)
        {
            string device_ip = this.ip_input.Text;
            string port = this.port_input.Text;
            var adbinfo = ADB.Connect($"{device_ip}:{port}");
            DeviceManager.Instance.UpdateDevices();
            Properties.Settings.Default.LastDeviceIP = device_ip;
            Properties.Settings.Default.Save();
            this.Close();
        }

      
        private void ChangeLang(string lang)
        {
            Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(lang);
            ApplyResource();
        }
        private void IPConnect_Load(object sender, EventArgs e)
        {
            this.ip_input.Text = Properties.Settings.Default.LastDeviceIP;
             

        }
        private void button1_Click(object sender, EventArgs e)
        {
            string device_ip = this.ip_input.Text;
            string port = this.port_input.Text;

            var p = Scrcpy.IPConnet(device_ip, port);
            if (p == null)
            {
                MessageBox.Show("连接失败 检查连接参数");
                return;
            }
            DeviceManager.Instance.BindDevicesProcees(device_ip + ":" + port, p);
            Properties.Settings.Default.LastDeviceIP = device_ip;
            Properties.Settings.Default.Save();


            this.Close();

        }
        // 遍历控件，并根据资源文件替换相应属性
        private void ApplyResource()
        {
            foreach (Control ctl in this.Controls)
            {
                resources.ApplyResources(ctl, ctl.Name);
            }
            this.ResumeLayout(false);
            this.PerformLayout();
            resources.ApplyResources(this, "$this");
            
        }

     
    }
}
