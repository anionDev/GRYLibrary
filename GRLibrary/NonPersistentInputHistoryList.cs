﻿using System.Collections.Generic;
namespace GRLibrary
{
    public class NonPersistentInputHistoryList
    {
        private List<string> UserInputs = new List<string>();
        private int CurrentuserInputIndex = 0;
        public void EnterPressed(string input)
        {
            this.UserInputs.Add(input.Trim());
            ResetCurrentReadPosition();
        }
        public string UpPressed()
        {
            if (this.CurrentuserInputIndex > 0)
            {
                this.CurrentuserInputIndex = this.CurrentuserInputIndex - 1;
            }
            return GetCurrentItem();
        }
        public string DownPressed()
        {
            if (this.CurrentuserInputIndex < this.UserInputs.Count)
            {
                this.CurrentuserInputIndex = this.CurrentuserInputIndex + 1;
            }
            return GetCurrentItem();
        }
        public void ResetCurrentReadPosition()
        {
            this.CurrentuserInputIndex = this.UserInputs.Count;
        }
        private string GetCurrentItem()
        {
            if (this.CurrentuserInputIndex == this.UserInputs.Count || this.UserInputs.Count == 0)
            {
                return string.Empty;
            }
            return this.UserInputs[this.CurrentuserInputIndex];
        }
    }
}
