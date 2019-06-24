﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using McsaMeetsMailer.Models;
using McsaMeetsMailer.Utils.HtmlBuilder;

namespace McsaMeetsMailer.Services
{
  public class FullScheduleEmailBuilder
  {
    public static string Build(IEnumerable<MeetDetailsModel> meetDetails )
    {
      var htmlBuilder = new HtmlBuilder();
      htmlBuilder.StartTable();

      var headings = new List<string>
      {
        "Leader",
        "Leader Email"
      };

      headings.AddRange( meetDetails.FirstOrDefault()?.AdditionalFields.Keys );

      htmlBuilder.AddHeadingRow( headings );

      foreach(var meetDetail in meetDetails )
      {
        var values = new List<string>
        {
          meetDetail.Leader,
          meetDetail.LeaderEmail
        };

        values.AddRange( meetDetail.AdditionalFields.Values );

        htmlBuilder.AddRow( values );
      }

      htmlBuilder.EndTable();

      return htmlBuilder.GetHtml();
    }
  }
}
