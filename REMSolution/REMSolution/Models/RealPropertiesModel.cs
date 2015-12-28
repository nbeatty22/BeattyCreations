using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.ComponentModel.DataAnnotations;

namespace REMSolution.Models
{

    public class FullProperty
    {

        public int PropertyID { get; set; }
        public string PropertyName { get; set; }
        public string AddressLine1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public string RecordSource { get; set; }
        public decimal? ListPrice { get; set; }
        public string PropertyType { get; set; }
        public string PropIcon { get; set; }
        public string DefaultImgUrl { get; set; }

    }

    public class RealPropertiesModel
    {

        public List<FullProperty> PropertyList { get; set; }
        public int PropertyID { get; set; }
        public string PropertyName { get; set; }

        [Display(Name = "Address Line 1")]
        public string AddressLine1 { get; set; }
        
        [Display(Name = "Address Line 2")]
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        
        [Display(Name = "Zip Code")]
        public string ZipCode { get; set; }
        public int? RecordSourceID { get; set; }
        public decimal? ListPrice { get; set; }
        public decimal? Zestimate { get; set; }
        public int? PropertyTypeID { get; set; }
        public string DefaultImgUrl { get; set; }
        public HttpPostedFileBase DefaultImageUpload { get; set; }
        public List<Image> PropertyPics { get; set; }
        public string SquareFeet { get; set; }
        public string Bedrooms { get; set; }
        public string Bathrooms { get; set; }
        public string Garage { get; set; }
        public string Basement { get; set; }
        public List<SelectListItem> PropertyTypeList { get; set; }
        public List<SelectListItem> RecordSourceList { get; set; }


        public void GetMapProperties()
        {
            var db = new RemingtonEntities();

            PropertyList = db.RealProperties.GroupJoin(db.Images.Where(x => x.PropDefault == true), Property => Property.PropertyID, Image => Image.PropertyId, (p, i) => new { Property = p, Image = i }).SelectMany( pi => pi.Image.DefaultIfEmpty(), (p, i) => new {Property = p.Property, Image = i})
                
                .Select(x => new FullProperty
            {
                PropertyID = x.Property.PropertyID,
                PropertyName = x.Property.PropertyName,
                AddressLine1 = x.Property.AddressLine1,
                City = x.Property.City,
                State = x.Property.State,
                Zip = x.Property.ZipCode,
                RecordSource = x.Property.RecordSource.RecordSourceName,
                ListPrice = x.Property.ListPrice,
                PropertyType = x.Property.PropertyType.PropertyTypeName,
                PropIcon = x.Property.PropertyType.Icon,
                DefaultImgUrl = x.Image.ImageUrl
            }).ToList();

        }

        public void GetSpecificProperty(int PropId)
        {
            var db = new RemingtonEntities();

            var PropertyRecord = db.RealProperties.Where(p => p.PropertyID == PropId).FirstOrDefault();

            PropertyID = PropertyRecord.PropertyID;
            PropertyName = PropertyRecord.PropertyName;
            AddressLine1 = PropertyRecord.AddressLine1;
            AddressLine2 = PropertyRecord.AddressLine2;
            City = PropertyRecord.City;
            State = PropertyRecord.State;
            ZipCode = PropertyRecord.ZipCode;
            RecordSourceID = PropertyRecord.RecordSourceID;
            ListPrice = PropertyRecord.ListPrice;
            Zestimate = PropertyRecord.Zestimate;
            PropertyTypeID = PropertyRecord.PropertyTypeId;
            PropertyPics = db.Images.Where(x => x.PropDefault == false && x.PropertyId == PropId).ToList();
            SquareFeet = PropertyRecord.SquareFeet;
            Bedrooms = PropertyRecord.Bedrooms;
            Bathrooms = PropertyRecord.Bathrooms;
            Garage = PropertyRecord.Garage;
            Basement = PropertyRecord.Basement;

            PropertyTypeList = db.PropertyTypes.Select(x => new SelectListItem { Text = x.PropertyTypeName, Value = x.PropertyTypeId.ToString() }).ToList();
            RecordSourceList = db.RecordSources.Select(x => new SelectListItem { Text = x.RecordSourceName, Value = x.RecordSourceID.ToString() }).ToList();

            var PropertyImages = db.Images.Where(i => i.PropertyId == PropId && i.PropDefault).FirstOrDefault();

            if (PropertyImages == null) 
            { 
                DefaultImgUrl = ""; 
            }
            else
            { 
                DefaultImgUrl = PropertyImages.ImageUrl ?? "";
            }

        }

        public void SavePropertyUpdate(RealPropertiesModel model)
        {
            var db = new RemingtonEntities();
            
            var PropertyRecord = db.RealProperties.Where(p => p.PropertyID == model.PropertyID).FirstOrDefault();

            PropertyRecord.PropertyName = model.PropertyName;
            PropertyRecord.AddressLine1 = model.AddressLine1;
            PropertyRecord.AddressLine2 = model.AddressLine2;
            PropertyRecord.City = model.City;
            PropertyRecord.State = model.State;
            PropertyRecord.ZipCode = model.ZipCode;
            PropertyRecord.RecordSourceID = model.RecordSourceID;
            PropertyRecord.ListPrice = model.ListPrice;
            PropertyRecord.Zestimate = model.Zestimate;
            PropertyRecord.PropertyTypeId = model.PropertyTypeID;
            PropertyRecord.SquareFeet = model.SquareFeet;
            PropertyRecord.Bedrooms = model.Bedrooms;
            PropertyRecord.Bathrooms = model.Bathrooms;
            PropertyRecord.Garage = model.Garage;
            PropertyRecord.Basement = model.Basement;

            db.SaveChanges();

            var PropertyImage = db.Images.Where(i => i.PropertyId == model.PropertyID && i.PropDefault).FirstOrDefault();

            if (PropertyImage == null)
            {
                PropertyImage = new Image();
                PropertyImage.PropDefault = true;
                PropertyImage.PropertyId = model.PropertyID;

                db.Images.Add(PropertyImage);
                
            }

            if (model.DefaultImgUrl != null)
            { 
                PropertyImage.ImageUrl = model.DefaultImgUrl;
            }

            db.SaveChanges();
            
        }
    
        public void AddPropertyPic(string path, int PropertyId)
        {

            var db = new RemingtonEntities();
            
            var PropertyImage = new Image();

            PropertyImage.ImageUrl = path;
            PropertyImage.PropDefault = false;
            PropertyImage.PropertyId = PropertyId;

            db.Images.Add(PropertyImage);

            db.SaveChanges();           

        }
    
    }
}