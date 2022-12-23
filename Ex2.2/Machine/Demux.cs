using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleComponents
{
    public class Demux : Gate, Component
    {
        public Wire Output1 { get; private set; }
        public Wire Output2 { get; private set; }
        public Wire Input { get; private set; }
        public Wire Control { get; private set; }

        public Demux()
        {
            Input = new Wire();
            Output1 = new Wire();
            Output2 = new Wire();
            Control = new Wire();
            Input.ConnectOutput(this);
            Control.ConnectOutput(this);
        }


        public override bool TestGate()
        {
            Control.Value = 0;
            Input.Value = 1;
            
            if (Output1.Value != Input.Value)
                return false;
            Input.Value = 0;
            
            if (Output1.Value != Input.Value)
                return false;

            Control.Value = 1;
            Input.Value = 1;
            
            if (Output2.Value != Input.Value)
                return false;
            Input.Value = 0;
            
            if (Output2.Value != Input.Value)
                return false; 
            return true;
        }

        #region Component Members

        public void Compute()
        {
            if (Control.Value == 0)
            {
                Output1.Value = Input.Value;
                Output2.Value = 0;
            }
            else
            {
                Output2.Value = Input.Value;
                Output1.Value = 0;
            }
        }

        #endregion
    }
}
