using Common.Utilities;
using Data;
using Data.Contracts;
using Entities.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Drawing.Printing;
using System.Runtime.Intrinsics.Arm;
using WebFramework.Filtters;

namespace Host.Controllers
{
    [ApiResultFilter]
    [ApiController]
    [AllowAnonymous]
    [Route("api/[controller]")]
    public class SendDocController : ControllerBase
    {
        private readonly IRepository<Contact> _contactRepository;
        private readonly IRepository<ContactPSPDocument> _contactPSPDocumentRepository;
        private readonly IRepository<UserCrm > _userCrmRepository;
        private readonly IRepository<Reception> _receptionRepository;
        private readonly IRepository<ContactPSPDocumentUrl> _contactpspDocumentUrlRepository;
        private readonly IRepository<vw_MultiContactPspAccount> _vw_MultiContactPspAccountRepository;
        private readonly IRepository<PSPDocument> _PSPDocumentRepository;
        private readonly IConfiguration _configuration;

        public SendDocController(

            IRepository<Contact> _contactRepository,
            IRepository<ContactPSPDocument> _contactPSPDocumentRepository,
            IRepository<UserCrm> _userCrmRepository,
            IRepository<Reception> _receptionRepository,
            IRepository<ContactPSPDocumentUrl> _contactpspDocumentUrlRepository,
            IRepository<vw_MultiContactPspAccount> _vw_MultiContactPspAccountRepository,
            IRepository<PSPDocument> _PSPDocumentRepository, IConfiguration configuration
            )
        {
            this._contactRepository = _contactRepository;
            this._contactPSPDocumentRepository = _contactPSPDocumentRepository;
            this._userCrmRepository = _userCrmRepository;
            this._receptionRepository = _receptionRepository;
            this._contactpspDocumentUrlRepository = _contactpspDocumentUrlRepository;
            this._vw_MultiContactPspAccountRepository = _vw_MultiContactPspAccountRepository;
            this._PSPDocumentRepository = _PSPDocumentRepository;
            _configuration = configuration;
        }
        public class IndexInputViewModel
        {
            public string? NationalCode { get; set; }
            public int? PageSize { get; set; } = 100;
            public string? Path { get; set; }
            public string? TerminalCode { get; set; }
            public string? ContactNationalCode { get; set; }
        }
        [HttpPost]
      public async Task<object> Index(IndexInputViewModel m , CancellationToken cancellationToken)
        {
            var listResult = new List<DocImageResult>();
            var doc = new ContactPSPDocument[0];

            var newDb = new NewCrmDbContext();


            if (m.NationalCode != null)
            {
                var rep = await _contactRepository.TableNoTracking.FirstOrDefaultAsync(c => c.NationalCode == m.NationalCode && c.Mode == 1);


                if (rep == null)
                {
                    rep = await _contactRepository.TableNoTracking.FirstOrDefaultAsync(c => c.NationalCode == m.NationalCode);

                    if (rep == null)
                        throw new Exception("هیچ نماینده یا بازاریابی با این کد ملی یافت نشد .");
                }

                var myValue = _configuration.GetValue<string>("CountDoc");

                var parameters = new[] {
            new SqlParameter("@CreatedBy", rep.Id),
            new SqlParameter("@a", myValue.ToInt() ),

        };

                doc = await newDb.ContactPSPDocuments.FromSqlRaw(@"
     SELECT TOP (@a) * FROM  CRM11_2.itnj.ContactPSPDocument AS cpd(NOLOCK)
      WHERE     NOT EXISTS (
        SELECT rpah.Id
        FROM  pub.ConvertLog   AS rpah (NOLOCK)
        WHERE rpah.EntityId = cpd.Id
      ) AND cpd.PSPId <> '0C048CF4-1BD1-41CB-BF42-DD0C2A1841A0' AND cpd.CreatedBy = @CreatedBy
   
   ", parameters).ToArrayAsync();

            }
            else if (m.TerminalCode != null)
            {

                doc = await _contactPSPDocumentRepository.TableNoTracking.Where(c => c.TerminalCode == m.TerminalCode).ToArrayAsync();

                if (doc.Length == 0)
                    throw new Exception("درخواست تخصیص با این ترمینال یافت نشد.");

                var exist = await newDb.ConvertLogs.FirstOrDefaultAsync(c => c.EntityId == doc[0].Id);

                if (exist != null)
                    throw new Exception("این درخواست تخصیص قبلا به سیستم اضافه شده است.");

            }
            else if (m.ContactNationalCode != null)
            {

                var contact = _contactRepository.TableNoTracking.Where(c => c.NationalCode == m.ContactNationalCode || c.EconomicCode == m.ContactNationalCode).Select(c => c.Id).FirstOrDefault();

                if (contact == null)
                    throw new Exception("مخاطبی با این کد ملی در سیستم قبلی یافت نشد.");

               var ldoc = await _contactPSPDocumentRepository.TableNoTracking.Where(c => c.ContactId == contact).ToListAsync();

                if (ldoc.Count == 0)
                {
                    throw new Exception("هیچ درخواست یافت نشد.");
                }

                var exist = await newDb.ConvertLogs.Where(c => ldoc.Select(c => c.Id).ToArray().Contains(c.EntityId) && !c.Message.Contains("مخاطب در سیستم جدید یافت نشد")).ToListAsync();


                if (exist.Count != 0)
                {
                    for (var i = 0; i < exist.Count; i++) {
                        var dr = ldoc.FirstOrDefault(z => z.Id == exist[i].Id);
                        if(dr != null) 
                         ldoc.Remove(dr);
                    }
                    
                }
                if(ldoc.Count == 0)
                    throw new Exception("درخواست های تخصیص این مخاطب قبلا به سیستم اضافه شده است.");

                doc = ldoc.ToArray();
            }
            else
            {
                
                var parameters = new[] {
            new SqlParameter("@a", m.PageSize)

        };

                doc = await newDb.ContactPSPDocuments.FromSqlRaw(@"
              SELECT TOP (@a) * FROM  CRM11_2.itnj.ContactPSPDocument AS cpd(NOLOCK)
            WHERE     NOT EXISTS (
                SELECT rpah.Id
                FROM  pub.ConvertLog   AS rpah (NOLOCK)
                WHERE rpah.EntityId = cpd.Id
              ) AND cpd.PSPId <> '0C048CF4-1BD1-41CB-BF42-DD0C2A1841A0' 
              ORDER BY CASE cpd.PSPId
	            WHEN '24C49818-7D9B-4345-AAB1-70692F1CFE42' THEN 1 
	            ELSE 2
	            END
   
   ", parameters).ToArrayAsync();
            }



            foreach (var i in doc)
            {
               

                    if (i == null)
                        throw new Exception("اطلاعات در سیستم یافت نشده بود.");

                    var docUrl = await _contactpspDocumentUrlRepository.TableNoTracking.Where(c=>c.ContactPSPDocumentId == i.Id).ToArrayAsync();


                    if (i.Contact == null)
                        i.Contact = await _contactRepository.GetByIdAsync(cancellationToken ,i.ContactId );


                if (i.Contact == null)
                    continue;


                    string? nationalCode = "";

                    var reception =  await _receptionRepository.TableNoTracking.FirstOrDefaultAsync(c => c.ContactPSPDocumentId == i.Id);
                        
                       


                    if (i.Contact.MainParent == null)
                    {
                        var visitor = await _contactRepository.GetByIdAsync(cancellationToken, i.CreatedBy);
                       

                        if (visitor == null && reception != null)
                        {
                            visitor = await _contactRepository.GetByIdAsync(cancellationToken, reception.CreatedBy);
                  
                            if (visitor == null)
                                nationalCode = "1400484355";
                        }
                        else
                            nationalCode = "1400484355";

                        if (visitor?.MainParent != null && string.IsNullOrWhiteSpace(nationalCode))
                            nationalCode =   _contactRepository.GetByIdAsync(cancellationToken, visitor.MainParent.Value)?.Result?.NationalCode;

                      

                        else if (reception?.CreatedBy != i.CreatedBy && string.IsNullOrWhiteSpace(nationalCode))
                        {
                            var user = new UserCrm();

                            if (reception == null)
                                user = await _userCrmRepository.GetByIdAsync(cancellationToken, reception?.CreatedBy);  
                            else
                                user = null;

                            if (user == null)
                            {
                                user = await _userCrmRepository.GetByIdAsync(cancellationToken, i?.CreatedBy);
                                if (user != null)
                                {
                                    nationalCode = _contactRepository.TableNoTracking.FirstOrDefault(c=>c.Id == i.CreatedBy)?.NationalCode;
                                   
                                }

                            }
                            else
                            {
                                nationalCode = _contactRepository.TableNoTracking.FirstOrDefault(c => c.Id == i.CreatedBy)?.NationalCode;
                               
                            }

                            if (string.IsNullOrWhiteSpace(nationalCode))
                            {
                              //  var parent =await _contactParentRepository.TableNoTracking .FirstOrDefaultAsync(c => c.ContactId == visitor.Id) ;
 
                                nationalCode = visitor.NationalCode;

                            }

                        }
                        else
                        {
                            if (visitor != null)
                                nationalCode = visitor?.NationalCode;

                        }


                    }
                    else
                    {
                        var p = _contactRepository.TableNoTracking.FirstOrDefault(c => c.Id == i.Contact.MainParent);
                        
                        nationalCode = p.NationalCode;
                    }


                    var neDoc = new DocImageResult();

                    neDoc = i.JsonSerialize().JsonDeserialize<DocImageResult>();

                    var ccc = _contactRepository.TableNoTracking.FirstOrDefault(c => c.Id == i.ContactId);
               

                    if (ccc.Type == 0)
                    {
                        neDoc.ContactNationalCode = ccc.NationalCode;
                    }
                    else
                    {
                        neDoc.ContactNationalCode = ccc.EconomicCode;
                    }


                    neDoc.CreatorNationalCode = nationalCode;
                    var rp = await _receptionRepository.TableNoTracking.Where(c => c.ContactPSPDocumentId == neDoc.Id).ToArrayAsync();
                         

                    if (rp.Any(c => c.Status == 50))
                    {
                        neDoc.Reception = rp.FirstOrDefault(c => c.Status == 50);
                    }
                    else
                    {
                        neDoc.Reception = rp.FirstOrDefault();
                    }


                    if (neDoc.ContactAccountId == null)
                    {

                        var acdd = await  _vw_MultiContactPspAccountRepository.TableNoTracking.Where(c => c.ContactPspDocumentId == neDoc.Id).ToArrayAsync();

 

                        neDoc.AccountIds = acdd.Select(c => c.ContactAccountId).ToArray();

                    }

                    neDoc.images = docUrl.JsonSerialize().JsonDeserialize<List<ImagesDoc>>();



                    listResult.Add(neDoc);


                    foreach (var d in listResult.Last().images)
                    {
                        try
                        {
                            d.DocumentId = (Guid)_PSPDocumentRepository.TableNoTracking.Where(c => c.Id == d.PSPDocumentId).FirstOrDefault().TitleId;

                            var byteData = System.IO.File.ReadAllBytes($@"D:\CRM\APP{d.Url}");
                            d.ContentByte = byteData ?? new byte[0];
                        }
                        catch(Exception e)
                            {
                        d.ContentByte =   new byte[0];
                    }
                        
                    }

                    for(var z =0; z <= listResult.Last().images.Count; z++)
                    {
                        if (listResult.Last().images.Count > z && listResult.Last().images[z].ContentByte.Length ==0 )
                        {
                          listResult.Last().images.Remove(listResult.Last().images[z]);
                        }
                    }
                    




            }



            return listResult;
        }

        [HttpPost("{action}")]
      public async Task<string> GetPhoto (IndexInputViewModel m)
        {
            var r = await System.IO.File.ReadAllBytesAsync($@"D:\CRM\APP{m.Path}");
            return   Convert.ToBase64String(r);
        } 
    }
}
