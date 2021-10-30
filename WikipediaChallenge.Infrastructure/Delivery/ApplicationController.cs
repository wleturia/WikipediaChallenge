﻿using System.Collections.Generic;
using WikipediaChallenge.Domain.VO;
using WikipediaChallenge.Domain.Mapping;
using System;
using System.Linq;

namespace WikipediaChallenge.Infrastructure.Delivery
{
    public class ApplicationController
    {
        readonly Application.Usecase.Application application;
        PageViewMapping mapping = new PageViewMapping();

        public ApplicationController(Application.Usecase.Application app)
        {
            application = app;
        }

        public void GetDataFromLastHours(int hours)
        {
            if (hours < 1)
            {
                Console.WriteLine("Hour cannot be less than 1");
                return;
            }

            List<DateTime> datetimes = new();

            Enumerable.Range(0, hours).ToList().ForEach(hour =>
            {
                datetimes.Add(DateTime.Now.AddHours(hour * -1));
            });

            List<Domain.DTO.WikipediaPageView> wikipediaPageViewsDTO = application.FromDateTimeListRetrieveWikipediaPageViewDTOList(datetimes);

            wikipediaPageViewsDTO.ForEach(wko =>
            {
                try
                {
                    Exception folder = application.localRepository.CreateFolder(wko.folder);
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                    return;
                }

                if (application.localRepository.VerifyFile(wko.folder + wko.filename))
                {
                    Console.WriteLine(String.Format("File {0} exists", wko.filename));
                    return;
                }
                application.wikipediaRepository.DownloadDataWikipediaDTO(wko);
            });
            /*
                        (List<Domain.Entity.PageView> pages, Exception err) = application.GetPageViewForPreviuosHours(hours);

                        if (err != null)
                        {
                            Console.WriteLine("The data cannot be retrieved: " + err.Message);
                            return;
                        }

                        IEnumerable<PageView> pageViews = mapping.MapPageViewFromModelToDTOList(pages);
                        ConsoleTables.ConsoleTable.From<PageView>(pageViews).Write();*/
            return;
        }
    }
}