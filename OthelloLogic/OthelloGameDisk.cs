namespace Ex05.OthelloLogic
{
    public struct OthelloGameDisk
    {
        private eCurrentDiskMode m_OthelloDisk;

        public eCurrentDiskMode DiskColor
        {
            get
            {
                return m_OthelloDisk;
            }

            set
            {
                m_OthelloDisk = value;
            }
        }

        public OthelloGameDisk(eCurrentDiskMode i_OthelloDisk)
        {
            m_OthelloDisk = i_OthelloDisk;
        }

        public eCurrentDiskMode GetOppositeDisk()
        {
            eCurrentDiskMode returnValue = eCurrentDiskMode.NotExist;
            if (DiskColor == eCurrentDiskMode.White)
            {
                returnValue = eCurrentDiskMode.Black;
            }
            else if (DiskColor == eCurrentDiskMode.Black)
            {
                returnValue = eCurrentDiskMode.White;
            }

            return returnValue;
        }
    }
}
