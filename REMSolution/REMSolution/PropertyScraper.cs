using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.Xml;
using System.Text.RegularExpressions;

namespace REMSolution
{
    public class PropertyScraper
    {
        public bool RealCompOnline (string FullUrl)
        {

            string s = "";

            using (WebClient client = new WebClient())
            {
                s = client.DownloadString(FullUrl);
            }

            //scrape html for applicable stuff

            MatchCollection m = Regex.Matches(s, @"(<TABLE class=""d163m2"" cellSpacing=0 cellPadding=0>.*?</TABLE>)", RegexOptions.Singleline);

            string AddressLine1 = "";
            string City = "";
            string State = "MI";
            string Zip = "xxxxx";

            var db = new RemingtonEntities();

            foreach (Match ms in m)
            {
                MatchCollection m2 = Regex.Matches(ms.Groups[1].Value, @"(<TR>.*?</TR>)", RegexOptions.Singleline);

                //Cycle through each listing and find the address elements
                foreach (Match m2s in m2)
                {
                    Match Addr1Match = Regex.Match(m2s.Groups[1].Value, @"<TD class=""d163m12""><span class=""formula field d163m11"">.*?</span></TD>", RegexOptions.Singleline);
                    AddressLine1 = Addr1Match.Groups[0].Value.Replace(@"<TD class=""d163m12""><span class=""formula field d163m11"">", "");
                    AddressLine1 = AddressLine1.Replace(@"</span></TD>", "");

                    Match CityMatch = Regex.Match(m2s.Groups[1].Value, @"<TD class=""d163m13""><span class=""field"">.*?</span></TD>", RegexOptions.Singleline);
                    City = CityMatch.Groups[0].Value.Replace(@"<TD class=""d163m13""><span class=""field"">", "");
                    City = City.Replace(@"</span></TD>", "");

                    //Lookup the record in the db and add it if it's not there
                    var Props = db.RealProperties.Where(x => x.AddressLine1 == AddressLine1).FirstOrDefault();

                    if (Props == null)
                    {

                        var NewProp = new RealProperty();

                        NewProp.AddressLine1 = AddressLine1;
                        NewProp.City = City;
                        NewProp.State = State;
                        NewProp.ZipCode = Zip;
                        NewProp.PropertyName = AddressLine1;
                        NewProp.RecordSourceID = 1;

                        db.RealProperties.Add(NewProp);

                    }

                    db.SaveChanges();

                }

            }

            //add error handling later
            return true;

        }

        public bool RealtorDotCom(string FullUrl)
        {
            string s = "";

            using (WebClient client = new WebClient())
            {
                s = client.DownloadString(FullUrl);
            }

            //scrape html for applicable stuff

            MatchCollection m = Regex.Matches(s, @"(<body.*?</body>)", RegexOptions.Singleline);

            string AddressLine1 = "";
            string City = "";
            string State = "MI";
            string Zip = "";
            string priceString = "0";
            decimal price = 0;

            var db = new RemingtonEntities();

            foreach (Match ms in m)
            {
                MatchCollection m2 = Regex.Matches(ms.Groups[1].Value, @"(<ul class=""listing-summary"">.*?</ul>)", RegexOptions.Singleline);

                //Cycle through each listing and find the address elements
                foreach (Match m2s in m2)
                {
                    Match Addr1Match = Regex.Match(m2s.Groups[1].Value, @"<span class=""listing-street-address"" itemprop=""streetAddress"">.*?</span>", RegexOptions.Singleline);
                    AddressLine1 = Addr1Match.Groups[0].Value.Replace(@"<span class=""listing-street-address"" itemprop=""streetAddress"">", "");
                    AddressLine1 = AddressLine1.Replace(@"</span>", "");

                    Match CityMatch = Regex.Match(m2s.Groups[1].Value, @"<span class=""listing-city"" itemprop=""addressLocality"">.*?</span>", RegexOptions.Singleline);
                    City = CityMatch.Groups[0].Value.Replace(@"<span class=""listing-city"" itemprop=""addressLocality"">", "");
                    City = City.Replace(@"</span>", "");

                    Match ZipMatch = Regex.Match(m2s.Groups[1].Value, @"<span class=""listing-postal"" itemprop=""postalCode"">.*?</span>", RegexOptions.Singleline);
                    Zip = ZipMatch.Groups[0].Value.Replace(@"<span class=""listing-postal"" itemprop=""postalCode"">", "");
                    Zip = Zip.Replace(@"</span>", "");

                    Match priceMatch = Regex.Match(m2s.Groups[1].Value, @"<span class=""listing-price blocker""><i class=""i-price-reduced""></i>.*?</span>", RegexOptions.Singleline);
                    priceString = priceMatch.Groups[0].Value.Replace(@"<span class=""listing-price blocker""><i class=""i-price-reduced""></i>$", "");
                    priceString = priceString.Replace(@"</span>", "");

                    try
                    {
                        price = Decimal.Parse(priceString);
                    }
                    catch (Exception e) 
                    {
                        price = 0;
                    }
                    

                    //Lookup the record in the db and add it if it's not there
                    var Props = db.RealProperties.Where(x => x.AddressLine1 == AddressLine1).FirstOrDefault();

                    if (Props == null)
                    {

                        var NewProp = new RealProperty();

                        NewProp.AddressLine1 = AddressLine1;
                        NewProp.City = City;
                        NewProp.State = State;
                        NewProp.ZipCode = Zip;
                        NewProp.PropertyName = AddressLine1;
                        NewProp.RecordSourceID = 2;
                        NewProp.ListPrice = price;

                        
                        
                        db.RealProperties.Add(NewProp);

                    }

                    db.SaveChanges();

                }

            }

            //add error handling later


            return true;
        }
    }
}