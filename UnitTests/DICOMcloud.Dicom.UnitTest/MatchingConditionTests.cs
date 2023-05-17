using System;
using System.Collections.Generic;
using System.Linq;
using Dicom;
using DICOMcloud.DataAccess.Matching;
using DICOMcloud.UnitTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DICOMcloud.Dicom.UnitTest
{
    [TestClass]
    public class MatchingConditionTests
    {
        DicomHelpers Helper { get; set; }
        [TestInitialize]
        public void Initialize()
        {
            Helper = new DicomHelpers();
        }

        [TestMethod]
        public void TestMatchingBase()
        {
            MatchingBase testTarget = new MatchingBase();
            //basic constructor test
            Assert.IsTrue(testTarget.Elements.SequenceEqual(new List<DicomItem>()));
            Assert.IsTrue(testTarget.SupportedTags.SequenceEqual(new List<uint>()));
            Assert.IsTrue(testTarget.ExactMatch);
            Assert.AreEqual(testTarget.KeyTag, (uint)0);

            DicomItem element = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsTrue(testTarget.CanMatch(element));
            Assert.IsTrue(testTarget.IsSupported(element));

            testTarget.SupportedTags.Add((uint)DicomTag.PatientNotProperlyFixatedQuantity);
            Assert.IsFalse(testTarget.CanMatch(element));
            Assert.IsFalse(testTarget.IsSupported(element));

            testTarget.SupportedTags.Add((uint)DicomTag.PatientName);
            Assert.IsTrue(testTarget.CanMatch(element));
            Assert.IsTrue(testTarget.IsSupported(element));
        }

        [TestMethod]
        public void TestSingleValueMatching()
        {
            SingleValueMatching testTarget = new SingleValueMatching();

            Assert.IsFalse(testTarget.IsCaseSensitive);
            Assert.IsTrue(testTarget.ExactMatch);

            DicomItem testSQelement = new DicomSequence(new DicomTag(0x0001, 0x0002), new DicomDataset());
            Assert.IsFalse(testTarget.CanMatch(testSQelement));

            DicomItem testDateelement = new DicomDate(new DicomTag(0x0001, 0x0003), "20001010");
            DicomItem testDateTimeelement = new DicomDateTime(new DicomTag(0x0001, 0x0004), "20001010090845.519");
            DicomItem testDateTime = new DicomTime(new DicomTag(0x0001, 0x0005), "090845.519");
            Assert.IsTrue(testTarget.CanMatch(testDateelement));
            Assert.IsTrue(testTarget.CanMatch(testDateTimeelement));
            Assert.IsTrue(testTarget.CanMatch(testDateTime));

            DicomItem wrongtestDateelement = new DicomDate(new DicomTag(0x0001, 0x0003), "2000-10-10");
            DicomItem wrongtestDateTimeelement = new DicomDateTime(new DicomTag(0x0001, 0x0004), "2000-10-10-09-08-45.519");
            DicomItem wrongtestDateTime = new DicomTime(new DicomTag(0x0001, 0x0005), "09-08-45.519");
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateelement));
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateTimeelement));
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateTime));

            DicomItem element = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsTrue(testTarget.CanMatch(element));

            DicomItem emptyElement = new DicomPersonName(DicomTag.PatientName, "");
            Assert.IsFalse(testTarget.CanMatch(emptyElement));

            DicomItem elementW1 = new DicomPersonName(DicomTag.PatientName, "testElem*");
            Assert.IsFalse(testTarget.CanMatch(elementW1));

            DicomItem elementW2 = new DicomPersonName(DicomTag.PatientName, "?testElem");
            Assert.IsFalse(testTarget.CanMatch(elementW2));
        }

        [TestMethod]
        public void TestListofUIDMatching()
        {
            ListofUIDMatching testTarget = new ListofUIDMatching();

            DicomItem otherElement = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsFalse(testTarget.CanMatch(otherElement));

            DicomItem singleUiElement = new DicomUniqueIdentifier(new DicomTag(0x0001, 0x0001), "testUIDelem");
            Assert.IsFalse(testTarget.CanMatch(singleUiElement));

            string[] uids = { "testUIDelem", "testUIDelem2" };
            DicomItem multiUiElement = new DicomUniqueIdentifier(new DicomTag(0x0001, 0x0001), uids);
            Assert.IsTrue(testTarget.CanMatch(multiUiElement));
        }

        [TestMethod]
        public void TestUniversalMatching()
        {
            UniversalMatching testTarget = new UniversalMatching();

            DicomItem testSQelement = new DicomSequence(new DicomTag(0x0001, 0x0002), new DicomDataset());
            Assert.IsFalse(testTarget.CanMatch(testSQelement));

            string[] uids = { "testUIDelem", "testUIDelem2" };
            DicomItem multiUiElement = new DicomUniqueIdentifier(new DicomTag(0x0001, 0x0001), uids);
            Assert.IsFalse(testTarget.CanMatch(multiUiElement));

            DicomItem otherElement = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsFalse(testTarget.CanMatch(otherElement));

            DicomItem emptyElement = new DicomPersonName(DicomTag.PatientName, "");
            Assert.IsTrue(testTarget.CanMatch(emptyElement));
        }

        [TestMethod]
        public void TestWildCardMatching()
        {
            WildCardMatching testTarget = new WildCardMatching();
            Assert.IsTrue(testTarget.IsCaseSensitive);
            Assert.IsFalse(testTarget.ExactMatch);

            DicomItem testSQelement = new DicomSequence(new DicomTag(0x0001, 0x0002), new DicomDataset());
            Assert.IsFalse(testTarget.CanMatch(testSQelement));

            DicomItem element = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsFalse(testTarget.CanMatch(element));

            DicomItem emptyElement = new DicomPersonName(DicomTag.PatientName, "");
            Assert.IsFalse(testTarget.CanMatch(emptyElement));

            DicomItem elementW1 = new DicomPersonName(DicomTag.PatientName, "testElem*");
            Assert.IsTrue(testTarget.CanMatch(elementW1));

            DicomItem elementW2 = new DicomPersonName(DicomTag.PatientName, "?testElem");
            Assert.IsTrue(testTarget.CanMatch(elementW2));
        }

        [TestMethod]
        public void TestRangeMatching()
        {
            RangeMatching testTarget = new RangeMatching();

            DicomItem patientElement = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsFalse(testTarget.CanMatch(patientElement));

            DicomItem testDateelement = new DicomDate(new DicomTag(0x0001, 0x0003), "20001010");
            DicomItem testDateTimeelement = new DicomDateTime(new DicomTag(0x0001, 0x0004), "20001010090845.519");
            DicomItem testDateTime = new DicomTime(new DicomTag(0x0001, 0x0005), "090845.519");
            Assert.IsTrue(testTarget.CanMatch(testDateelement));
            Assert.IsTrue(testTarget.CanMatch(testDateTimeelement));
            Assert.IsTrue(testTarget.CanMatch(testDateTime));

            DicomItem wrongtestDateelement = new DicomDate(new DicomTag(0x0001, 0x0003), "2000*10*10");
            DicomItem wrongtestDateTimeelement = new DicomDateTime(new DicomTag(0x0001, 0x0004), "2000-10?10-09*08-45.519");
            DicomItem wrongtestDateTime = new DicomTime(new DicomTag(0x0001, 0x0005), "09?08*45.519");
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateelement));
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateTimeelement));
            Assert.IsFalse(testTarget.CanMatch(wrongtestDateTime));

        }

        [TestMethod]
        public void TestSequenceMatching()
        {
            SequenceMatching testTarget = new SequenceMatching();

            DicomItem testSQelement = new DicomSequence(new DicomTag(0x0001, 0x0002), new DicomDataset());
            Assert.IsTrue(testTarget.CanMatch(testSQelement));

            DicomItem element = new DicomPersonName(DicomTag.PatientName, "testElem");
            Assert.IsFalse(testTarget.CanMatch(element));
        }
        
    }
}
