﻿// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using Common;
using Windows.UI.Xaml.Tests.MUXControls.InteractionTests.Infra;
using Windows.UI.Xaml.Tests.MUXControls.InteractionTests.Common;
using System.Collections.Generic;

#if USING_TAEF
using WEX.TestExecution;
using WEX.TestExecution.Markup;
using WEX.Logging.Interop;
#else
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;
#endif
using Microsoft.Windows.Apps.Test.Automation;
using Microsoft.Windows.Apps.Test.Foundation;
using Microsoft.Windows.Apps.Test.Foundation.Controls;
using Microsoft.Windows.Apps.Test.Foundation.Patterns;
using Microsoft.Windows.Apps.Test.Foundation.Waiters;

namespace Windows.UI.Xaml.Tests.MUXControls.InteractionTests
{
    [TestClass]
    public class NumberBoxTests
    {
        [ClassInitialize]
        [TestProperty("RunAs", "User")]
        [TestProperty("Classification", "Integration")]
        [TestProperty("Platform", "Any")]
        [TestProperty("MUXControlsTestSuite", "SuiteB")]
        public static void ClassInitialize(TestContext testContext)
        {
            TestEnvironment.Initialize(testContext);
        }

        public void TestCleanup()
        {
            TestCleanupHelper.Cleanup();
        }

        [TestMethod]
        public void UpDownTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");
                Verify.AreEqual(0, numBox.Value);

                ComboBox spinModeComboBox = FindElement.ByName<ComboBox>("SpinModeComboBox");
                spinModeComboBox.SelectItemByName("Inline");
                Wait.ForIdle();

                Button upButton = FindButton(numBox, "Increase");
                Button downButton = FindButton(numBox, "Decrease");

                Log.Comment("Verify that up button increases value by 1");
                upButton.InvokeAndWait();
                Verify.AreEqual(1, numBox.Value);

                Log.Comment("Verify that down button decreases value by 1");
                downButton.InvokeAndWait();
                Verify.AreEqual(0, numBox.Value);

                Log.Comment("Change Step value to 5");
                RangeValueSpinner stepNumBox = FindElement.ByName<RangeValueSpinner>("StepNumberBox");
                stepNumBox.SetValue(5);
                Wait.ForIdle();

                Log.Comment("Verify that up button increases value by 5");
                upButton.InvokeAndWait();
                Verify.AreEqual(5, numBox.Value);

                Check("MinCheckBox");
                Check("MaxCheckBox");

                numBox.SetValue(98);
                Wait.ForIdle();
                Log.Comment("Verify that when wrapping is off, clicking the up button won't go past the max value.");
                upButton.InvokeAndWait();
                Verify.AreEqual(100, numBox.Value);

                Check("WrapCheckBox");

                Log.Comment("Verify that when wrapping is on, clicking the up button wraps to the min value.");
                upButton.InvokeAndWait();
                Verify.AreEqual(0, numBox.Value);

                Uncheck("WrapCheckBox");

                Log.Comment("Verify that when wrapping is off, clicking the down button won't go past the min value.");
                downButton.InvokeAndWait();
                Verify.AreEqual(0, numBox.Value);

                Check("WrapCheckBox");

                Log.Comment("Verify that when wrapping is on, clicking the down button wraps to the max value.");
                downButton.InvokeAndWait();
                Verify.AreEqual(100, numBox.Value);
            }
        }

        [TestMethod]
        public void MinMaxTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                Check("MinCheckBox");
                Check("MaxCheckBox");

                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");
                numBox.SetValue(10);
                Wait.ForIdle();

                Log.Comment("Verify that setting the value to -1 changes the value to 0");
                numBox.SetValue(-1);
                Wait.ForIdle();
                Verify.AreEqual(0, numBox.Value);

                Log.Comment("Verify that typing '123' in the NumberBox changes the value to 100");
                EnterText(numBox, "123");
                Verify.AreEqual(100, numBox.Value);

                Log.Comment("Changing Max to 90; verify value also changes to 90");
                EnterText(FindElement.ByName<RangeValueSpinner>("MaxNumberBox"), "90");
                Verify.AreEqual(90, numBox.Value);
            }
        }

        [TestMethod]
        public void BasicKeyboardTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");

                Log.Comment("Verify that loss of focus validates textbox contents");
                EnterText(numBox, "8", false);
                KeyboardHelper.PressKey(Key.Tab);
                Wait.ForIdle();
                Verify.AreEqual(8, numBox.Value);

                Log.Comment("Verify that pressing escape cancels entered text");
                EnterText(numBox, "3", false);
                KeyboardHelper.PressKey(Key.Escape);
                Wait.ForIdle();
                Verify.AreEqual(8, numBox.Value);

                Log.Comment("Verify that pressing up arrow increases the value");
                KeyboardHelper.PressKey(Key.Up);
                Wait.ForIdle();
                Verify.AreEqual(9, numBox.Value);

                Log.Comment("Verify that pressing down arrow decreases the value");
                KeyboardHelper.PressKey(Key.Down);
                Wait.ForIdle();
                Verify.AreEqual(8, numBox.Value);
            }
        }

        [TestMethod]
        public void GamepadTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");

                Log.Comment("Verify that pressing A validates textbox contents");
                EnterText(numBox, "8", false);
                GamepadHelper.PressButton(numBox, GamepadButton.A);
                Wait.ForIdle();
                Verify.AreEqual(8, numBox.Value);

                Log.Comment("Verify that pressing B cancels entered text");
                EnterText(numBox, "3", false);
                GamepadHelper.PressButton(numBox, GamepadButton.B);
                Wait.ForIdle();
                Verify.AreEqual(8, numBox.Value);
            }
        }

        [TestMethod]
        public void ScrollTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");

                Check("HyperScrollCheckBox");

                InputHelper.RotateWheel(numBox, 1);
                InputHelper.RotateWheel(numBox, 1);
                Wait.ForIdle();
                Verify.AreEqual(2, numBox.Value);

                InputHelper.RotateWheel(numBox, -1);
                InputHelper.RotateWheel(numBox, -1);
                InputHelper.RotateWheel(numBox, -1);
                Wait.ForIdle();
                Verify.AreEqual(-1, numBox.Value);
            }
        }

        [TestMethod]
        public void CustomFormatterTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");
                numBox.SetValue(8);
                Wait.ForIdle();
                Edit edit = FindTextBox(numBox);
                Verify.AreEqual("8", edit.GetText());

                Button button = FindElement.ByName<Button>("CustomFormatterButton");
                button.InvokeAndWait();

                Verify.AreEqual("8.00", edit.GetText());
            }
        }

        [TestMethod]
        public void CoersionTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                ComboBox validationComboBox = FindElement.ByName<ComboBox>("ValidationComboBox");
                validationComboBox.SelectItemByName("Disabled");
                Wait.ForIdle();

                Check("MinCheckBox");
                Check("MaxCheckBox");

                Log.Comment("Verify that numbers outside the range can be set if validation is disabled.");
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");
                numBox.SetValue(-10);
                Wait.ForIdle();
                Verify.AreEqual(-10, numBox.Value);

                numBox.SetValue(150);
                Wait.ForIdle();
                Verify.AreEqual(150, numBox.Value);
            }
        }

        [TestMethod]
        public void BasicCalculationTest()
        {
            using (var setup = new TestSetupHelper("NumberBox Tests"))
            {
                RangeValueSpinner numBox = FindElement.ByName<RangeValueSpinner>("TestNumberBox");

                Log.Comment("Verify that calculations don't work if AcceptsCalculations is false");
                EnterText(numBox, "5 + 3");
                Verify.AreEqual(0, numBox.Value);

                Check("CalculationCheckBox");

                int numErrors = 0;
                const double resetValue = 1234;
                
                Dictionary<string, double> expressions = new Dictionary<string, double>
                {
                    // Valid expressions. None of these should evaluate to the reset value.
                    { "5", 5 },
                    { "-358", -358 },
                    { "12.34", 12.34 },
                    { "5 + 3", 8 },
                    { "12345 + 67 + 890", 13302 },
                    { "000 + 0011", 11 },
                    { "5 - 3 + 2", 4 },
                    { "3 + 2 - 5", 0 },
                    { "9 - 2 * 6 / 4", 6 },
                    { "9 - -7",  16 },
                    { "9-3*2", 3 },         // no spaces
                    { " 10  *   6  ", 60 }, // extra spaces
                    { "10 /( 2 + 3 )", 2 },
                    { "5 * -40", -200 },
                    { "(1 - 4) / (2 + 1)", -1 },
                    { "3 * ((4 + 8) / 2)", 18 },
                    { "23 * ((0 - 48) / 8)", -138 },
                    { "((74-71)*2)^3", 216 },
                    { "2 - 2 ^ 3", -6 },
                    { "2 ^ 2 ^ 2 / 2 + 9", 17 },
                    { "5 ^ -2", 0.04 },
                    { "5.09 + 14.333", 19.423 },
                    { "2.5 * 0.35", 0.875 },
                    { "-2 - 5", -7 },       // begins with negative number
                    { "(10)", 10 },         // number in parens
                    { "(-9)", -9 },         // negative number in parens
                    { "0^0", 1 },           // who knew?

                    // These should not parse, which means they will reset back to the previous value.
                    { "5x + 3y", resetValue },        // invalid chars
                    { "5 + (3", resetValue },         // mismatched parens
                    { "9 + (2 + 3))", resetValue },
                    { "(2 + 3)(1 + 5)", resetValue }, // missing operator
                    { "9 + + 7", resetValue },        // extra operators
                    { "9 - * 7",  resetValue },
                    { "9 - - 7",  resetValue },
                    { "+9", resetValue },
                    { "1 / 0", resetValue },          // divide by zero

                    // These don't currently work, but maybe should.
                    { "-(3 + 5)", resetValue }, // negative sign in front of parens -- should be -8
                };
                foreach (KeyValuePair<string, double> pair in expressions)
                {
                    numBox.SetValue(resetValue);
                    Wait.ForIdle();

                    EnterText(numBox, pair.Key);
                    string output = "Expression '" + pair.Key + "' - expected: " + pair.Value + ", actual: " + numBox.Value;
                    if (Math.Abs(pair.Value - numBox.Value) > 0.00001)
                    {
                        numErrors++;
                        Log.Warning(output);
                    }
                    else
                    {
                        Log.Comment(output);
                    }
                }

                Verify.AreEqual(0, numErrors);
            }
        }

        Button FindButton(UIObject parent, string buttonName)
        {
            foreach (UIObject elem in parent.Children)
            {
                if (elem.Name.Equals(buttonName))
                {
                    return new Button(elem);
                }
            }
            Log.Comment("Did not find " + buttonName + " button for object " + parent.Name);
            return null;
        }

        Edit FindTextBox(UIObject parent)
        {
            foreach (UIObject elem in parent.Children)
            {
                if (elem.ClassName.Equals("TextBox"))
                {
                    return new Edit(elem);
                }
            }
            Log.Comment("Did not find TextBox for object " + parent.Name);
            return null;
        }

        void Check(string checkboxName)
        {
            CheckBox checkBox = FindElement.ByName<CheckBox>(checkboxName);
            checkBox.Check();
            Wait.ForIdle();
        }

        void Uncheck(string checkboxName)
        {
            CheckBox checkBox = FindElement.ByName<CheckBox>(checkboxName);
            checkBox.Uncheck();
            Wait.ForIdle();
        }

        void EnterText(RangeValueSpinner numBox, string text, bool pressEnter = true)
        {
            Edit edit = FindTextBox(numBox);
            if (edit != null)
            {
                KeyboardHelper.EnterText(edit, text);
                if (pressEnter)
                {
                    KeyboardHelper.PressKey(Key.Enter);
                }
                Wait.ForIdle();
            }
        }
    }
}
