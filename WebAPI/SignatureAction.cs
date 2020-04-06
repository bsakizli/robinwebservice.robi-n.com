using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Models;


namespace WebAPI
{
    public class SignatureModel
    {
        public int TicketId { get; set; }
        public int FormId { get; set; }

        public bool IsSignature { get; set; }
    }


    public class GetSignatureModel
    {
        public int FormId { get; set; }
    }


    public class SignatureAction : BaseDAL
    {


        public SignatureModels AddSignature(SignatureModel model)
        {
            try
            {

                var Record = RobinDB.RBN_EMPTOR_SIGNATURE.Where(X => X.FormId == model.FormId).Count();

                if (Record > 0)
                {
                    //Kayıt Var Sadece Güncelle

                    var UpdateRecord = RobinDB.RBN_EMPTOR_SIGNATURE.Where(X => X.FormId == model.FormId).First();
                    UpdateRecord.Signature = model.IsSignature;
                    if (RobinDB.SaveChanges() == 1)
                    {
                        var Item = RobinDB.RBN_EMPTOR_SIGNATURE.Where(X => X.FormId == model.FormId).FirstOrDefault();

                        SignatureModels signatureModels = new SignatureModels()
                        {
                            IsCode = true,
                            Message = "Gerekli olan işaretleme yapılmıştır.",
                            Results = new Result
                            {
                                SignatureId = Item.Id,
                                FormId = Convert.ToInt32(Item.FormId)
                            }
                        };

                        return signatureModels;
                    } else
                    {
                        SignatureModels signatureModels = new SignatureModels()
                        {
                            IsCode = false,
                            Message = "Gerekli olan işaretleme güncellemesi yapılırken bir sorun meydana geldi."
                        };

                        return signatureModels;
                    }

                } else
                {
                    int XXX = Convert.ToInt32(model.FormId);

                    var SignatureAdd = RobinDB.Set<RBN_EMPTOR_SIGNATURE>();
                    SignatureAdd.Add(new RBN_EMPTOR_SIGNATURE {
                    Active=true,
                    FormId = model.FormId,
                    Signature = model.IsSignature,
                    TicketId = model.TicketId,
                    RegisterDate = DateTime.Now
                    });
                    if (RobinDB.SaveChanges() == 1)
                    {
                        var Item = RobinDB.RBN_EMPTOR_SIGNATURE.Where(X => X.FormId == model.FormId).FirstOrDefault();

                       

                        SignatureModels signatureModels = new SignatureModels()
                        {
                            IsCode = true,
                            Message = "Gerekli olan işaretleme yapılmıştır.",
                            Results = new Result
                            {
                                SignatureId = Item.Id,
                                FormId = Convert.ToInt32(Item.FormId)
                            }
                        };

                        return signatureModels;
                    } else
                    {
                        SignatureModels signatureModels = new SignatureModels()
                        {
                            IsCode = false,
                            Message = "Gerekli olan işaretleme ekleme yapılırken bir sorun meydana geldi."
                        };

                        return signatureModels;
                    }

                }

            } catch
            {

                SignatureModels signatureModels = new SignatureModels()
                {
                    IsCode = false,
                    Message = "Gerekli olan işaretleme sırasında bir hata meydana geldi. Servis Sorunu"
                };

                return signatureModels;

            }
        }


        public GetSignatureActive GetSignature(GetSignatureModel getSignatureModel)
        {
            try
            {
                var Record = RobinDB.RBN_EMPTOR_SIGNATURE.Where(X => X.Active == true).Where(X => X.FormId == getSignatureModel.FormId).FirstOrDefault();


                GetSignatureActive getSignatureActive = new GetSignatureActive()
                {
                    IsCode = true,
                    Message = "Servis formu bilgisi başarıyla listelenmiştir",
                    Items = new Signature()
                    {
                        SignatureActive = Convert.ToBoolean(Record.Signature),
                        SignatureId = Convert.ToInt32(Record.FormId)
                    }
                };

                return getSignatureActive;

            } catch
            {
                GetSignatureActive getSignatureActive = new GetSignatureActive()
                {
                    IsCode = false,
                    Message = "Servis formu bilgisi sırasında servis formu hatası oluştur. Web service hatası"
                };
                return getSignatureActive;
            }
        }

    }
}
