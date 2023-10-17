using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;

namespace Entities.CRM
{
    public enum ContactType
    {
        Real,
        Legal
    }
    public enum ContactMode
    {
        [Description("مشتری")]
        Customer,
        [Description("بازاریاب")]
        Visitor,
        [Description("نماینده")]
        Representative,
        [Description("کارمند")]
        Personel,
        [Description("تامین کننده")]
        Provider,
        [Description("معرف")]
        Introducer,
        [Description("معرف ویژه")]
        SpecialIntroducer
    }
    public enum ContactStatus
    {
        [Description("")]
        None,
        [Description("در صف بررسی")]
        Waiting,
        [Description("تایید شده")]
        Approved,
        [Description("رد شده")]
        Rejected
    }
    public class vw_MultiContactPspAccount      
    {
        public bool? IsMultiAccountOwner { get; set; }

        public Guid? ContactAccountId { get; set; }

        public Guid? ContactId { get; set; }

        public Guid? ContactPspDocumentId { get; set; }

        public int? BankCode { get; set; }

        public string? BranchCode { get; set; }

        public string? Number { get; set; }

        public string? ShebaCode { get; set; }

        public string? CardNumber { get; set; }

        public bool? IsMain { get; set; }
    }
    public class ContactParent  
    {
        [Required]
        public DateTime CreatedOn { get; set; }

        [Required]
        [ForeignKey("Contact")]
       
        public Guid ContactId { get; set; }

        [Required]
        [ForeignKey("Parent")]
        
        public Guid ParentId { get; set; }

        [Required]
        public Guid CreatedBy { get; set; }

        public Guid? MainParent { get; set; }

        public Contact Contact { get; set; }

        public Contact Parent { get; set; }

 
    }
    public class UserCrm 
    {
        [Key]
        public Guid Id { get; set; }

        public string?   Username { get; set; }


        public string?   Password { get; set; }

        public string? Email { get; set; }

        public string? Firstname { get; set; }


        public string? Lastname { get; set; }


        public string? MobileNumber { get; set; }

        public string? ImageUrl { get; set; }

        public bool? IsEnabled { get; set; }

        public bool? CanChangePassword { get; set; }

    
        public bool? ChangePasswordFirstLogin { get; set; }


        public bool? IsLogin { get; set; }

       
        public DateTime? ExpirationDate { get; set; }

        
        public DateTime? CreationDate { get; set; }

        public DateTime? UpdateTime { get; set; }



    }

        public class DocImageResult : ContactPSPDocument
    {

        public List<ImagesDoc> images { get; set; }
        public string ContactNationalCode { get; set; }
        public string CreatorNationalCode { get; set; }
        public Reception? Reception { get; set; }
        public Guid?[] AccountIds { get; set; } = new Guid?[0];
    }

    public class ImagesDoc : ContactPSPDocumentUrl
    {
        public byte[] ContentByte { get; set; }
        public Guid UrlId { get; set; }
        public Guid? SpDocumentId { get; set; }
        public Guid DocumentId { get; set; }

    }

    public class Reception
    {


        public bool? HasHologram
        {
            get;
            set;
        }


        public byte  ? Type
        {
            get;
            set;
        }

        public byte? Status
        {
            get;
            set;
        }


        public DateTime? CreatedOn
        {
            get;
            set;
        }

        [Key]
        public Guid Id
        {
            get;
            set;
        }


        public Guid? CreatedBy
        {
            get;
            set;
        }


        public Guid? ContactProductId
        {
            get;
            set;
        }


        public Guid? ContactId
        {
            get;
            set;
        }

        public Guid? OwnerId
        {
            get;
            set;
        }

        public string? Number
        {
            get;
            set;
        }

        public Guid? TransferTypeId
        {
            get;
            set;
        }

        public int? TransferAmount
        {
            get;
            set;
        }

        public string? TransferNumber
        {
            get;
            set;
        }

        public string? Description
        {
            get;
            set;
        }

        public string? ShopName
        {
            get;
            set;
        }


        public Guid? PSPId
        {
            get;
            set;
        }

        public Guid? DeviceModelId
        {
            get;
            set;
        }


        public Guid? DeviceGradeId
        {
            get;
            set;
        }

        public Guid? ProductRevisionId
        {
            get;
            set;
        }

        public string? DeviceSerialNumber
        {
            get;
            set;
        }

        public string? HardwareSerialNumber
        {
            get;
            set;
        }

        public string? SerialNumber
        {
            get;
            set;
        }


        public Guid? ReplacedPSPId
        {
            get;
            set;
        }

        public Guid? ReplacedDeviceModelId
        {
            get;
            set;
        }


        public Guid? ReplacedDeviceGradeId
        {
            get;
            set;
        }

        public Guid? ReplacedProductRevisionId
        {
            get;
            set;
        }

        public string? ReplacedDeviceSerialNumber
        {
            get;
            set;
        }

        public string? ReplacedHardwareSerialNumber
        {
            get;
            set;
        }

        public string? ReplacedSerialNumber
        {
            get;
            set;
        }

        public DateTime? GuarantyDate
        {
            get;
            set;
        }

        public Guid? ContactPSPDocumentId
        {
            get;
            set;
        }

        public byte? TypeofAssignment
        {
            get;
            set;
        }

        public Guid? ReplaceAndRollbackReasonID
        {
            get;
            set;
        }

        public string? TerminalCode
        {
            get;
            set;
        }

        public string? MerchantCode
        {
            get;
            set;
        }

        public string? PSPTrackNumber
        {
            get;
            set;
        }

        public string? ErrorMessage
        {
            get;
            set;
        }

        public string? IMEI
        {
            get;
            set;
        }

        public string? Mobile
        {
            get;
            set;
        }

    }

    public class ContactPSPDocumentUrl
    {
        public byte? Status
        {
            get;
            set;
        }

        public Guid? Id
        {
            get;
            set;
        }

        public Guid? ContactPSPDocumentId
        {
            get;
            set;
        }

        public Guid? PSPDocumentId
        {
            get;
            set;
        }


        public string? Url
        {
            get;
            set;
        }

        public string? Description
        {
            get;
            set;
        }

        public string? RejectDescription
        {
            get;
            set;
        }

        public bool? SendToPsp
        {
            get;
            set;
        }
    }

    public class Contact
    {

        public DateTime? Birthdate
        {
            get;
            set;
        }


        public DateTime? CreatedOn
        {
            get;
            set;
        }

        [Key]
        public Guid? Id
        {
            get;
            set;
        }


        public Guid? EducationId
        {
            get;
            set;
        }


        public Guid? CreatedBy
        {
            get;
            set;
        }


        public Guid? ProvinceId
        {
            get;
            set;
        }


        public Guid? CityId
        {
            get;
            set;
        }


        public Guid? ContactDocumentTeamId
        {
            get;
            set;
        }


        public Guid? SurnameId
        {
            get;
            set;
        }


        public Guid? ActivityId
        {
            get;
            set;
        }


        public Guid? GuildId
        {
            get;
            set;
        }


        public Guid? SubGuildId
        {
            get;
            set;
        }


        public Guid? ContactId
        {
            get;
            set;
        }



        public Guid? MainParent
        {
            get;
            set;
        }



        public string? NationalCode
        {
            get;
            set;
        }


        public string? Code
        {
            get;
            set;
        }



        public string? Name
        {
            get;
            set;
        }

        public string? CodeRaivarz
        {
            get;
            set;
        }


        public string? Lastname
        {
            get;
            set;
        }


        public string? Fathername
        {
            get;
            set;
        }


        public string? ComOwnerName
        {
            get;
            set;
        }


        public string? ComOwnerLastName
        {
            get;
            set;
        }


        public string? ImageUrl
        {
            get;
            set;
        }


        public string? IDCard
        {
            get;
            set;
        }


        public string? EconomicCode
        {
            get;
            set;
        }


        public DateTime? RegisterDate
        {
            get;
            set;
        }


        public string? RegisterNumber
        {
            get;
            set;
        }


        public byte? Type
        {
            get;
            set;
        }


        public byte? Mode
        {
            get;
            set;
        }

        public byte? Status
        {
            get;
            set;
        }


        public Guid? ExpertId
        {
            get;
            set;
        }


        public bool? Authenticated
        {
            get;
            set;
        }


        public bool? SimAuthenticated
        {
            get;
            set;
        }


        public string? Description
        {
            get;
            set;
        }



        public string? ForeignersPervasiveCode
        {
            get;
            set;
        }

        public string? PassportCode
        {
            get;
            set;
        }


        public DateTime? PassportCreditDate
        {
            get;
            set;
        }


        public Guid? Country
        {
            get;
            set;
        }

        public bool? ISForeignNationals
        {
            get;
            set;
        }


        public DateTime? BusinessLicenseEndDate
        {
            get;
            set;
        }





    }

    
    public class ContactPSPDocument
    {
        public Contact? Contact
        {
            get;
            set;
        }

        public bool? Approved
        {
            get;
            set;
        }

        public bool? IsConverted
        {
            get;
            set;
        }

        public bool? Authenticate
        {
            get;
            set;
        }

        public bool? IntroduceSpecial
        {
            get;
            set;
        }


        public byte? Status
        {
            get;
            set;
        }


        public int? DeviceQty
        {
            get;
            set;
        }


        public DateTime? CreatedOn
        {
            get;
            set;
        }

        public DateTime? AssignToExpertOn
        {
            get;
            set;
        }


        public Guid? Id
        {
            get;
            set;
        }

        public Guid?  ContactId
        {
            get;
            set;
        }
        public Guid? ContactShopId
        {
            get;
            set;
        }

        public Guid? PSPId
        {
            get;
            set;
        }


        public Guid? ExpertId
        {
            get;
            set;
        }


        public Guid? CreatedBy
        {
            get;
            set;
        }


        public int? Number
        {
            get;
            set;
        }


        public Guid? ContactAccountId
        {
            get;
            set;
        }

        public Guid? IntroduceId
        {
            get;
            set;
        }

        public string? ReferenceCode
        {
            get;
            set;
        }

        public DateTime? ReferenceDate
        {
            get;
            set;
        }

        public int? ReferenceAmount
        {
            get;
            set;
        }


        public string? PSPTrackNumber
        {
            get;
            set;
        }


        public bool? TestNumberIsFromWebService
        {
            get;
            set;
        }


        public string? TaxNumber
        {
            get;
            set;
        }


        public string? SinaTrackNumber
        {
            get;
            set;
        }


        public string? TerminalCode
        {
            get;
            set;
        }


        public string? MerchantCode
        {
            get;
            set;
        }

        public string? Message
        {
            get;
            set;
        }

        public string? HardwareSerialNumber
        {
            get;
            set;
        }

        public DateTime? RegisterToPsp
        {
            get;
            set;
        }

        public DateTime? GetTerminal
        {
            get;
            set;
        }

        public DateTime? LastTimeSave
        {
            get;
            set;
        }

        public DateTime? SendImageToPspDate
        {
            get;
            set;
        }

        public bool? ImageApproved
        {
            get;
            set;
        }


    }

    public class PSPDocument  
    {
 
        public bool? IsActive { get; set; }

       
        public bool? IsRequired { get; set; }
 
        public string? Code { get; set; }

        [Key]
        public Guid Id { get; set; }

 
        public Guid? PSPId { get; set; }

 
        public Guid? TitleId { get; set; }

 
  
    }

    public class  ConvertLog
{
    public Guid? Id { get; set; }

    public DateTime? CreatedOn { get; set; }

    public Guid? CreatedBy { get; set; }

    public DateTime? ModifiedOn { get; set; }

    public Guid? ModifiedBy { get; set; }

    public Guid? OwnerId { get; set; }

    public int? IsActive { get; set; }

    public bool? Success { get; set; }

    public Guid? EntityId { get; set; }

    public int? Type { get; set; }

    public string Message { get; set; }

    public DateTime? EntityCreatedOn { get; set; }

    public Guid? NewEntityId { get; set; }

}


}
