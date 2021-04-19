using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.IO;
using Data.Business;
using log4net;
using Quartz;
using Quartz.Impl;
using Data.Utils;

namespace APIProject.Job
{
    public class Jobclass : IJob
    {
        /// <summary>
        /// Hàm thực hiện trong tiến trình
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        /// 

        //Tiến trình 1 : Cộng điểm hằng ngày cho customer 
        //- Mỗi customer sau khi mua sản phẩm sẽ được hoàn lại 80% số điểm vào ví điểm tích
        //- Mỗi ngày sẽ hoàn lại 0,2% vào ví point và sau 120 ngày sẽ xuống 0,1%
        public async Task CheckTaskService()
        {
            System.Diagnostics.Debug.WriteLine("job1");
            ILog log = log4net.LogManager.GetLogger(typeof(Jobclass));
            TaskBusiness taskBus = new TaskBusiness();
            taskBus.SaveLog("Schuder Task  start");
            //Tiến trình hoàn trả điểm hàng ngày
            taskBus.AddPointEveryDay();
            taskBus.MinusPointEveryMonth();
            log.Info("Schuder Task  start ");
            //Lấy tất cả user có đã có điểm tích lũy lớn hơn 0

           

        }

        public async Task SendNotySale()
        {
            System.Diagnostics.Debug.WriteLine("job2");

        }
        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.WhenAll(CheckTaskService());
            }
            catch
            {
                return;
            }
        }
    }

    public class JobMinusPoint : IJob
    {
        //Tiến trình 2: Trừ điểm hàng tháng với user là 20 điểm <=> 20..000 VNĐ
        //Với những tài khoản lập sau ngày 25 thì không trừ

        public async Task MinusPoint()
        {
            ILog log = log4net.LogManager.GetLogger(typeof(JobMinusPoint));
            TaskBusiness taskBus = new TaskBusiness();
            //Tiến trình thu phí sử dụng app hàng tháng
            taskBus.SaveLog("Schuder MinusPoint Task  start");
            //taskBus.MinusPointEveryMonth();
            log.Info("Schuder Task  start ");
            //Lấy tất cả user có đã có điểm tích lũy lớn hơn 0

            System.Diagnostics.Debug.WriteLine("job2");

        }

        public async Task Execute(IJobExecutionContext context)
        {
            try
            {
                await Task.WhenAll(MinusPoint());
            }
            catch
            {
                return;
            }
        }

    }



    //public class JobBirthday : IJob
    //{
    //    public async Task Execute(IJobExecutionContext context)
    //    {

    //    }

    //}
}