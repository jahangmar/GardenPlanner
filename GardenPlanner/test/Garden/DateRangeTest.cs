// Copyright (c) 2020 Jahangmar
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
using System;
using NUnit.Framework;
using GardenPlanner.Garden;

namespace GardenPlanner.test.Garden
{
    [TestFixture]
    public class DateRangeTest
    {
        public DateRangeTest()
        {
        }

        [Test]
        public void ConstrDateRangeZeroArgs()
        {
            DateRange dateRange = new DateRange();
            Assert.AreEqual(1, dateRange.GetStart().Day);
            Assert.AreEqual(1, dateRange.GetEnd().Day);
            Assert.AreEqual(1, dateRange.GetStart().Month);
            Assert.AreEqual(1, dateRange.GetEnd().Month);
            Assert.Less(dateRange.GetStart().Year, 2000);
            Assert.Less(dateRange.GetEnd().Year, 2000);
            Assert.IsTrue(dateRange.IsDefault());
            Assert.IsTrue(dateRange.IsEmpty());
        }

        [Test]
        public void ConstrDateRangeYMD()
        {
            DateRange dateRange = new DateRange(2000, 2, 3, 2000, 2, 3);
            Assert.AreEqual(3, dateRange.GetStart().Day);
            Assert.AreEqual(3, dateRange.GetEnd().Day);
            Assert.AreEqual(2, dateRange.GetStart().Month);
            Assert.AreEqual(2, dateRange.GetEnd().Month);
            Assert.AreEqual(2000, dateRange.GetStart().Year);
            Assert.AreEqual(2000, dateRange.GetEnd().Year);
            Assert.IsFalse(dateRange.IsDefault());
            Assert.IsTrue(dateRange.IsEmpty());

            dateRange = new DateRange(2000, 2, 3, 1990, 1, 2);
            Assert.IsTrue(dateRange.IsEmpty());
        }

        [Test]
        public void ConstrDateRangeYM()
        {
            DateRange dateRange = new DateRange(2000, 2, 2000, 2);
            Assert.AreEqual(1, dateRange.GetStart().Day);
            Assert.AreEqual(1, dateRange.GetEnd().Day);
            Assert.AreEqual(2, dateRange.GetStart().Month);
            Assert.AreEqual(2, dateRange.GetEnd().Month);
            Assert.AreEqual(2000, dateRange.GetStart().Year);
            Assert.AreEqual(2000, dateRange.GetEnd().Year);
            Assert.IsFalse(dateRange.IsDefault());
            Assert.IsTrue(dateRange.IsEmpty());
        }

        [Test]
        public void GetRangeInDays()
        {
            DateRange dateRange = new DateRange(2000, 2, 3, 2000, 2, 3);
            Assert.AreEqual(0, dateRange.GetRangeInDays());

            dateRange = new DateRange(2000, 2, 3, 2000, 2, 4);
            Assert.AreEqual(1, dateRange.GetRangeInDays());

            dateRange = new DateRange(2020, 10, 1, 2020, 11, 4);
            Assert.AreEqual(31 + 3, dateRange.GetRangeInDays());
        }

        [Test]
        public void GetRangeInRoundMonths()
        {
            DateRange dateRange = new DateRange(2000, 1, 1, 2001, 1, 1);
            Assert.AreEqual(12, dateRange.GetRangeInRoundMonths());

            dateRange = new DateRange(2000, 1, 1, 2000, 2, 1);
            Assert.AreEqual(1, dateRange.GetRangeInRoundMonths());

            dateRange = new DateRange(2000, 1, 1, 2000, 1, 1);
            Assert.AreEqual(0, dateRange.GetRangeInRoundMonths());

            dateRange = new DateRange(2000, 1, 1, 2000, 1, 20);
            Assert.AreEqual(1, dateRange.GetRangeInRoundMonths());
        }

        [Test]
        public void GetRangeInRoundYears()
        {
            DateRange dateRange = new DateRange(2000, 1, 1, 2001, 1, 1);
            Assert.AreEqual(1, dateRange.GetRangeInRoundYears());

            dateRange = new DateRange(2000, 1, 1, 2000, 1, 1);
            Assert.AreEqual(0, dateRange.GetRangeInRoundYears());

            dateRange = new DateRange(2000, 1, 1, 2000, 12, 1);
            Assert.AreEqual(1, dateRange.GetRangeInRoundYears());

            dateRange = new DateRange(2000, 1, 1, 2001, 7, 20);
            Assert.AreEqual(2, dateRange.GetRangeInRoundYears());
        }

        [Test]
        public void IsDateInRange()
        {
            DateRange dateRange = new DateRange(2000, 1, 1, 2001, 1, 1);
            Assert.IsTrue(dateRange.IsDateInRange(new DateTime(2000, 6, 1)));
            Assert.IsTrue(dateRange.IsDateInRange(2000, 6));
            Assert.IsTrue(dateRange.IsDateInRange(new DateTime(2000, 1, 1)));
            Assert.IsTrue(dateRange.IsDateInRange(2000, 1));
            Assert.IsTrue(dateRange.IsDateInRange(new DateTime(2001, 1, 1)));
            Assert.IsTrue(dateRange.IsDateInRange(2001, 1));

            Assert.IsFalse(dateRange.IsDateInRange(new DateTime(2001, 2, 1)));
            Assert.IsFalse(dateRange.IsDateInRange(2001, 2));
            Assert.IsFalse(dateRange.IsDateInRange(new DateTime(2002, 1, 1)));
            Assert.IsFalse(dateRange.IsDateInRange(2002, 1));
            Assert.IsFalse(dateRange.IsDateInRange(new DateTime(1999, 12, 1)));
            Assert.IsFalse(dateRange.IsDateInRange(1999, 12));


            dateRange = new DateRange(2000, 1, 1, 2000, 1, 1);
            Assert.IsFalse(dateRange.IsDateInRange(new DateTime(2000, 2, 1)));
            Assert.IsFalse(dateRange.IsDateInRange(2001, 2));

            Assert.IsTrue(dateRange.IsDateInRange(new DateTime(2000, 1, 1)));
            Assert.IsTrue(dateRange.IsDateInRange(2000, 1));
            Assert.IsFalse(dateRange.IsDateInRange(new DateTime(1999, 12, 1)));
            Assert.IsFalse(dateRange.IsDateInRange(1999, 12));
        }


    }
}
