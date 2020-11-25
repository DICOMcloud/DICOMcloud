using Dicom;
using Dicom.Log;
using Dicom.Network;
using DICOMcloud.DataAccess;
using DICOMcloud.DataAccess.Database;
using DICOMcloud.DataAccess.Database.Schema;
using DICOMcloud.IO;
using DICOMcloud.Media;
using DICOMcloud.Pacs;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DicomClient = Dicom.Network.Client.DicomClient;

namespace DICOMcloud.Wado.PacsInterface.PacsService.SCPReleated
{
    class SCP : DicomService, IDicomServiceProvider, 
        IAsyncDicomCEchoProvider, 
        IAsyncDicomCStoreProvider, 
        IDicomCFindProvider,
        IDicomCGetProvider,
        IDicomCMoveProvider
    {
        public SCP(INetworkStream stream, Encoding fallbackEncoding, Logger log)
                : base(stream, fallbackEncoding, log)
        {
            
        }

        private IStorageLocation RetrieveSopInstance(DicomDataset query)
        {
            var seriesInstanceUID = query.GetSingleValueOrDefault(DicomTag.SeriesInstanceUID, string.Empty);
            var sOPInstanceUID = query.GetSingleValueOrDefault(DicomTag.SOPInstanceUID, string.Empty);
            var studyInstanceUID = query.GetSingleValueOrDefault(DicomTag.StudyInstanceUID, string.Empty);

            IObjectId odjId = new ObjectId()
            {
                SeriesInstanceUID = seriesInstanceUID,
                SOPInstanceUID = sOPInstanceUID,
                StudyInstanceUID = studyInstanceUID
            };

            return PacsServer.RetrieveService.RetrieveSopInstance(odjId, new DicomMediaProperties(MimeMediaTypes.DICOM, "1.2.840.10008.1.2.1"));
        }

        private void AddSearchRequestData(DicomDataset src)
        {
            if (!src.Contains(DicomTag.StudyInstanceUID))
            {
                src.Add(new DicomUniqueIdentifier(DicomTag.StudyInstanceUID, ""));
            }
            if (!src.Contains(DicomTag.SeriesInstanceUID))
            {
                src.Add(new DicomUniqueIdentifier(DicomTag.SeriesInstanceUID, ""));
            }
            if (!src.Contains(DicomTag.SOPInstanceUID))
            {
                src.Add(new DicomUniqueIdentifier(DicomTag.SOPInstanceUID, ""));
            }
        }

        #region IDicomCMoveProviderImp

        public IEnumerable<DicomCMoveResponse> OnCMoveRequest(DicomCMoveRequest request)
        {
            //var cMoveAETitle = CADServer.DB.CMoveAETitles.Where(a => a.AETitle == request.DestinationAE).ToList();

            //// the c-move request contains the DestinationAE. the data of this AE should be configured somewhere.
            //if (cMoveAETitle.Count == 0)
            //{
            //    yield return new DicomCMoveResponse(request, DicomStatus.QueryRetrieveMoveDestinationUnknown);
            //    yield return new DicomCMoveResponse(request, DicomStatus.ProcessingFailure);
            //    yield break;
            //}

            //// this data should come from some data storage!
            //var destinationPort = cMoveAETitle[0].Port;
            //var destinationIP = cMoveAETitle[0].IP;
            var destinationPort = 11112;
            var destinationIP = "127.0.0.1";

            IDbCommand selectCommand = PacsServer.DatabaseService.CreateCommand();
            selectCommand.Connection = PacsServer.DatabaseService.CreateConnection();
            selectCommand.CommandText = string.Format(@"SELECT * From CMoveIPTable WHERE AEtitle = '{0}'", request.DestinationAE);

            selectCommand.Connection.Open();
            using (var reader = selectCommand.ExecuteReader(CommandBehavior.CloseConnection | CommandBehavior.SingleRow))
            {
                if (reader.Read())
                {
                    destinationPort = reader.GetInt32(2);
                    destinationIP = reader.GetString(1);
                    Console.WriteLine("{0} {1} {2} {3}", reader.GetInt64(0), reader.GetString(1), reader.GetInt32(2), reader.GetString(3));
                }
                else 
                {
                    yield return new DicomCMoveResponse(request, DicomStatus.QueryRetrieveMoveDestinationUnknown);
                    yield return new DicomCMoveResponse(request, DicomStatus.ProcessingFailure);
                    yield break;
                }
            }

            

            var queryLevel = request.Level;
            
            var matchingFiles = new List<IStorageLocation>();

            switch (queryLevel)
            {
                case DicomQueryRetrieveLevel.Patient:
                    {
                        IEnumerable<DicomDataset> resStudys = PacsServer.QueryService.FindStudies(request.Dataset, new QueryOptions());
                        foreach (var resStudy in resStudys)
                        {
                            var resSerices = PacsServer.QueryService.FindSeries(resStudy, new QueryOptions());
                            foreach (var resSerice in resSerices)
                            {
                                AddSearchRequestData(resSerice);
                                var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                                foreach (var resObject in resObjects)
                                {
                                    matchingFiles.Add(RetrieveSopInstance(resObject));
                                }
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Study:
                    {
                        IEnumerable<DicomDataset> resStudys = PacsServer.QueryService.FindStudies(request.Dataset, new QueryOptions());
                        foreach (var resStudy in resStudys)
                        {
                            var resSerices = PacsServer.QueryService.FindSeries(resStudy, new QueryOptions());
                            foreach (var resSerice in resSerices)
                            {
                                AddSearchRequestData(resSerice);
                                var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                                foreach (var resObject in resObjects)
                                {
                                    matchingFiles.Add(RetrieveSopInstance(resObject));
                                }
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Series:
                    {
                        var resSerices = PacsServer.QueryService.FindSeries(request.Dataset, new QueryOptions());
                        foreach (var resSerice in resSerices)
                        {
                            AddSearchRequestData(resSerice);
                            var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                            foreach (var resObject in resObjects)
                            {
                                matchingFiles.Add(RetrieveSopInstance(resObject));
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Image:
                    {
                        AddSearchRequestData(request.Dataset);
                        var resObjects = PacsServer.QueryService.FindObjectInstances(request.Dataset, new QueryOptions());
                        foreach (var resObject in resObjects)
                        {
                            matchingFiles.Add(RetrieveSopInstance(resObject));
                        }
                    }
                    break;
            }

            var client = new DicomClient(destinationIP, destinationPort, false, PacsServer.AETitle, request.DestinationAE);
            client.NegotiateAsyncOps();
            int storeTotal = matchingFiles.Count();
            int storeDone = 0; // this variable stores the number of instances that have already been sent
            int storeFailure = 0; // this variable stores the number of faulues returned in a OnResponseReceived
            foreach (IStorageLocation location in matchingFiles)
            {
                var storeRequest = new DicomCStoreRequest(location.ID);
                // !!! there is a Bug in fo-dicom 3.0.2 that the OnResponseReceived handlers are invoked not until the DicomClient has already
                //     sent all the instances. So the counters are not increased image by image sent but only once in a bulk after all storage
                //     has been finished. This bug will be fixed hopefully soon.
                storeRequest.OnResponseReceived += (req, resp) =>
                {
                    if (resp.Status == DicomStatus.Success)
                    {
                        Logger.Info("Storage of image successfull");
                        storeDone++;
                    }
                    else
                    {
                        Logger.Error("Storage of image failed");
                        storeFailure++;
                    }
                };
                client.AddRequestAsync(storeRequest).Wait();
            }

            var sendTask = client.SendAsync();

            while (!sendTask.IsCompleted)
            {
                // while the send-task is runnin we inform the QR SCU every 2 seconds about the status and how many instances are remaining to send. 
                yield return new DicomCMoveResponse(request, DicomStatus.Pending) { Remaining = storeTotal - storeDone - storeFailure, Completed = storeDone };
                Thread.Sleep(TimeSpan.FromSeconds(2));
            }

            Logger.Info("..finished");
            yield return new DicomCMoveResponse(request, DicomStatus.Success);
        }

        #endregion

        #region IDicomCGetProviderImp

        public IEnumerable<DicomCGetResponse> OnCGetRequest(DicomCGetRequest request)
        {
            var queryLevel = request.Level;

            switch (queryLevel)
            {
                case DicomQueryRetrieveLevel.Patient:
                    {
                        IEnumerable<DicomDataset> resStudys = PacsServer.QueryService.FindStudies(request.Dataset, new QueryOptions());
                        foreach (var resStudy in resStudys)
                        {
                            var resSerices = PacsServer.QueryService.FindSeries(resStudy, new QueryOptions());
                            foreach (var resSerice in resSerices)
                            {
                                AddSearchRequestData(resSerice);
                                var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                                foreach (var resObject in resObjects)
                                {
                                    var location = RetrieveSopInstance(resObject);
                                    var storeRequest = new DicomCStoreRequest(location.ID);
                                    SendRequestAsync(storeRequest).Wait();
                                }
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Study:
                    {
                        IEnumerable<DicomDataset> resStudys = PacsServer.QueryService.FindStudies(request.Dataset, new QueryOptions());
                        foreach (var resStudy in resStudys)
                        {
                            var resSerices = PacsServer.QueryService.FindSeries(resStudy, new QueryOptions());
                            foreach (var resSerice in resSerices)
                            {
                                AddSearchRequestData(resSerice);
                                var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                                foreach (var resObject in resObjects)
                                {
                                    var location = RetrieveSopInstance(resObject);
                                    var storeRequest = new DicomCStoreRequest(location.ID);
                                    SendRequestAsync(storeRequest).Wait();
                                }
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Series:
                    {
                        var resSerices = PacsServer.QueryService.FindSeries(request.Dataset, new QueryOptions());
                        foreach (var resSerice in resSerices)
                        {
                            AddSearchRequestData(resSerice);
                            var resObjects = PacsServer.QueryService.FindObjectInstances(resSerice, new QueryOptions());
                            foreach (var resObject in resObjects)
                            {
                                var location = RetrieveSopInstance(resObject);
                                var storeRequest = new DicomCStoreRequest(location.ID);
                                SendRequestAsync(storeRequest).Wait();
                            }
                        }
                    }
                    break;

                case DicomQueryRetrieveLevel.Image:
                    {
                        AddSearchRequestData(request.Dataset);
                        var resObjects = PacsServer.QueryService.FindObjectInstances(request.Dataset, new QueryOptions());
                        foreach (var resObject in resObjects)
                        {
                            var location = RetrieveSopInstance(resObject);
                            var storeRequest = new DicomCStoreRequest(location.ID);
                            SendRequestAsync(storeRequest).Wait();
                        }
                    }
                    break;
            }
            yield return new DicomCGetResponse(request, DicomStatus.Success);
        }

        #endregion

        #region IDicomCFindProviderImp

        public IEnumerable<DicomCFindResponse> OnCFindRequest(DicomCFindRequest request)
        {
            var queryLevel = request.Level;
            
            IEnumerable<DicomDataset> res = Enumerable.Empty<DicomDataset>();

            switch (queryLevel)
            {    
                case DicomQueryRetrieveLevel.Patient:
                    {
                        res = PacsServer.QueryService.FindStudies(request.Dataset,new QueryOptions());
                    }
                    break;

                case DicomQueryRetrieveLevel.Study:
                    {
                        res = PacsServer.QueryService.FindStudies(request.Dataset, new QueryOptions());
                    }
                    break;

                case DicomQueryRetrieveLevel.Series:
                    {
                        res = PacsServer.QueryService.FindSeries(request.Dataset, new QueryOptions());
                    }
                    break;

                case DicomQueryRetrieveLevel.Image:
                    {
                        res = PacsServer.QueryService.FindObjectInstances(request.Dataset, new QueryOptions());
                    }
                    break;
            }

            foreach (var dsResponse in res)
            {
                yield return new DicomCFindResponse(request, DicomStatus.Pending) { Dataset = dsResponse };
            }
            yield return new DicomCFindResponse(request, DicomStatus.Success);
        }

        #endregion

        #region IAsyncDicomCStoreProviderImp

        public Task<DicomCStoreResponse> OnCStoreRequestAsync(DicomCStoreRequest request)
        {
            DicomDataset dicomDs = request.File.Dataset;
            try
            {
                var result = PacsServer.StorageService.StoreDicom(dicomDs, new InstanceMetadata() { });

                return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.Success));
            }
            catch (Exception ex)
            {
                Logger.Log(LogLevel.Error, ex.Message.ToString());
                return Task.FromResult(new DicomCStoreResponse(request, DicomStatus.ProcessingFailure));
            }
        }

        public Task OnCStoreRequestExceptionAsync(string tempFileName, Exception e)
        {
            return Task.CompletedTask;
        }

        #endregion

        #region IAsyncDicomCEchoProviderImp

        public Task<DicomCEchoResponse> OnCEchoRequestAsync(DicomCEchoRequest request)
        {
            return Task.FromResult(new DicomCEchoResponse(request, DicomStatus.Success));
        }

        #endregion

        #region IDicomServiceProviderImp

        private static readonly DicomTransferSyntax[] AcceptedTransferSyntaxes = new DicomTransferSyntax[]
        {
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        private static readonly DicomTransferSyntax[] AcceptedImageTransferSyntaxes = new DicomTransferSyntax[]
        {
            // Lossless
            DicomTransferSyntax.JPEGLSLossless,
            DicomTransferSyntax.JPEG2000Lossless,
            DicomTransferSyntax.JPEGProcess14SV1,
            DicomTransferSyntax.JPEGProcess14,
            DicomTransferSyntax.RLELossless,
            // Lossy
            DicomTransferSyntax.JPEGLSNearLossless,
            DicomTransferSyntax.JPEG2000Lossy,
            DicomTransferSyntax.JPEGProcess1,
            DicomTransferSyntax.JPEGProcess2_4,
            // Uncompressed
            DicomTransferSyntax.ExplicitVRLittleEndian,
            DicomTransferSyntax.ExplicitVRBigEndian,
            DicomTransferSyntax.ImplicitVRLittleEndian
        };

        public void OnConnectionClosed(Exception exception)
        {
            //throw new NotImplementedException();
        }

        public void OnReceiveAbort(DicomAbortSource source, DicomAbortReason reason)
        {
            //throw new NotImplementedException();
        }

        public Task OnReceiveAssociationReleaseRequestAsync()
        {
            return SendAssociationReleaseResponseAsync();
        }

        public Task OnReceiveAssociationRequestAsync(DicomAssociation association)
        {
            string callingAE = association.CallingAE;
            string calledAE = association.CalledAE;
            if (PacsServer.AETitle != calledAE)
            {
                Logger.Error($"Association with {callingAE} rejected since called aet {calledAE} is unknown");
                return SendAssociationRejectAsync(
                    DicomRejectResult.Permanent,
                    DicomRejectSource.ServiceUser,
                    DicomRejectReason.CalledAENotRecognized);
            }

            //var result = CADServer.DB.AcceptAETitles.Where(a => a.AETitle == callingAE).ToList();

            //if (result.Count == 0)
            //{
            //    Logger.Error($"Association with {callingAE} rejected since calling aet {callingAE} is unknown");
            //    return SendAssociationRejectAsync(
            //        DicomRejectResult.Permanent,
            //        DicomRejectSource.ServiceUser,
            //        DicomRejectReason.CalledAENotRecognized);
            //}

            foreach (var pc in association.PresentationContexts)
            {
                if (pc.AbstractSyntax == DicomUID.Verification
                    || pc.AbstractSyntax == DicomUID.PatientRootQueryRetrieveInformationModelFIND
                    || pc.AbstractSyntax == DicomUID.PatientRootQueryRetrieveInformationModelMOVE
                    || pc.AbstractSyntax == DicomUID.StudyRootQueryRetrieveInformationModelFIND
                    || pc.AbstractSyntax == DicomUID.StudyRootQueryRetrieveInformationModelMOVE)
                {
                    pc.AcceptTransferSyntaxes(AcceptedTransferSyntaxes);
                }
                else if (pc.AbstractSyntax == DicomUID.PatientRootQueryRetrieveInformationModelGET
                    || pc.AbstractSyntax == DicomUID.StudyRootQueryRetrieveInformationModelGET)
                {
                    pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
                }
                else if (pc.AbstractSyntax.StorageCategory != DicomStorageCategory.None)
                {
                    pc.AcceptTransferSyntaxes(AcceptedImageTransferSyntaxes);
                }
                else
                {
                    Logger.Warn($"Requested abstract syntax {pc.AbstractSyntax} from {callingAE} not supported");
                    pc.SetResult(DicomPresentationContextResult.RejectAbstractSyntaxNotSupported);
                }
            }

            return SendAssociationAcceptAsync(association);
        }

        #endregion

    }
}
