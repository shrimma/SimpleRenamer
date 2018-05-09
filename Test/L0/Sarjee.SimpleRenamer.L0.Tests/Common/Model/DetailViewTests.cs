using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sarjee.SimpleRenamer.Common.Model;
using System;

namespace Sarjee.SimpleRenamer.L0.Tests.Common.Model
{
    [TestClass]
    public class DetailViewTests
    {
        private DetailView GetDetailView(string id = "id", string showName = "showName", string year = "year", string description = "description")
        {
            DetailView detailView = new DetailView(id, showName, year, description);
            detailView.Should().NotBeNull();
            return detailView;
        }

        #region Constructor
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void DetailViewCtor_InvalidArguments_ThrowArgumentNullExceptions()
        {
            Action action1 = () => new DetailView(string.Empty, string.Empty, string.Empty, string.Empty);
            Action action2 = () => new DetailView("id", string.Empty, string.Empty, string.Empty);
            Action action3 = () => new DetailView("id", "showName", string.Empty, string.Empty);
            Action action4 = () => new DetailView("id", "showName", "year", string.Empty);

            action1.Should().Throw<ArgumentNullException>();
            action2.Should().Throw<ArgumentNullException>();
            action3.Should().Throw<ArgumentNullException>();
            action4.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void DetailViewCtor_Success()
        {
            DetailView detailView = null;
            Action action1 = () => detailView = GetDetailView();

            action1.Should().NotThrow();
            detailView.Should().NotBeNull();

            detailView.Id.Should().Be("id");
            detailView.ShowName.Should().Be("showName");
            detailView.Year.Should().Be("year");
            detailView.Description.Should().Be("description");
        }
        #endregion Constructor

        #region Equality
        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void DetailView_Equality_Success()
        {
            //generic view
            DetailView detailView1 = GetDetailView();
            DetailView detailView6 = GetDetailView();

            //different id
            DetailView detailView2 = GetDetailView(id: "id2");

            //different showname
            DetailView detailView3 = GetDetailView(showName: "showName2");

            //different year
            DetailView detailView4 = GetDetailView(year: "year2");

            //different description
            DetailView detailView5 = GetDetailView(description: "description2");

            //comparison to self should be TRUE
            bool result1 = detailView1.Equals(detailView1);
            bool result2 = detailView1.Equals(detailView6);
            bool result10 = detailView1.Equals((object)detailView1);
            bool result11 = detailView1.Equals((object)detailView6);

            //other comparisons should be FALSE
            bool result3 = detailView1.Equals(detailView2);
            bool result4 = detailView1.Equals(detailView3);
            bool result5 = detailView1.Equals(detailView4);
            bool result6 = detailView1.Equals(detailView5);

            //comparison with null or invalid object should be FALSE
            bool result7 = detailView1.Equals(null);
            bool result8 = detailView1.Equals(new object());
            bool result9 = detailView1.Equals((object)null);

            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeFalse();
            result4.Should().BeFalse();
            result5.Should().BeFalse();
            result6.Should().BeFalse();
            result7.Should().BeFalse();
            result8.Should().BeFalse();
            result9.Should().BeFalse();
            result10.Should().BeTrue();
            result11.Should().BeTrue();
        }

        [TestMethod]
        [TestCategory(TestCategories.Common)]
        public void DetailView_GetHashCode_Success()
        {
            //generic view
            int detailView1 = GetDetailView().GetHashCode();
            int detailView6 = GetDetailView().GetHashCode();

            //different id
            int detailView2 = GetDetailView(id: "id2").GetHashCode();

            //different showname
            int detailView3 = GetDetailView(showName: "showName2").GetHashCode();

            //different year
            int detailView4 = GetDetailView(year: "year2").GetHashCode();

            //different description
            int detailView5 = GetDetailView(description: "description2").GetHashCode();

            //comparison to self should be TRUE
            bool result1 = detailView1.Equals(detailView1);
            bool result2 = detailView1.Equals(detailView6);

            //other comparisons should be FALSE
            bool result3 = detailView1.Equals(detailView2);
            bool result4 = detailView1.Equals(detailView3);
            bool result5 = detailView1.Equals(detailView4);
            bool result6 = detailView1.Equals(detailView5);

            result1.Should().BeTrue();
            result2.Should().BeTrue();
            result3.Should().BeFalse();
            result4.Should().BeFalse();
            result5.Should().BeFalse();
            result6.Should().BeFalse();
        }
        #endregion Equality
    }
}
