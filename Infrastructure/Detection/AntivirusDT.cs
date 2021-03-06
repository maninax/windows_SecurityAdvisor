﻿using SecurityAdvisor.Infrastructure.Generic;
using System;
using System.IO;
using System.Text;
using System.Threading;

namespace SecurityAdvisor.Infrastructure.Detection
{
    class AntivirusDT : DetectionTechnique
    {
        #region Consts
        private const string EICAR_STRING = @"X5O!P%@AP[4\PZX54(P^)7CC)7}$EICAR-STANDARD-ANTIVIRUS-TEST-FILE!$H+H*";
        private const string TEST_FILE_NAME = "eicar.txt";
        #endregion

        public override void Execute()
        {
            try
            {
                PlaceTestFile();
                CheckFileAvailability();
            }
            catch (UnauthorizedAccessException)
            {
                Status = DetectionStatus.NotFound; //расчёт на то, что именно антивирус блокирует доступ к файлу, а значит систему защищена
                //(такое возможно при повторном запуске программы уже после обнаружения антивирусом файла)
            }
        }

        private void PlaceTestFile()
        {
            File.WriteAllText(TEST_FILE_NAME, EICAR_STRING, Encoding.Default);
        }

        private void CheckFileAvailability()
        {
            int loop = 0;
            int loopMax = 5; //*3=кол-во секунд на ожидание

            do
            {
                Thread.Sleep(3000);
                loop ++;
            }
            while (IsUndetectedByAntivirus() && loop < loopMax);

            Status = (loop == loopMax) ? DetectionStatus.Found : DetectionStatus.NotFound;

            bool IsUndetectedByAntivirus() => File.Exists(TEST_FILE_NAME) && new FileInfo(TEST_FILE_NAME).Length != 0;
        }
    }
}
