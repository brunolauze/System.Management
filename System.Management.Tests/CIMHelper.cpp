//%LICENSE////////////////////////////////////////////////////////////////
//
// Licensed to The Open Group (TOG) under one or more contributor license
// agreements.  Refer to the OpenPegasusNOTICE.txt file distributed with
// this work for additional information regarding copyright ownership.
// Each contributor licenses this file to you under the OpenPegasus Open
// Source License; you may not use this file except in compliance with the
// License.
//
// Permission is hereby granted, free of charge, to any person obtaining a
// copy of this software and associated documentation files (the "Software"),
// to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense,
// and/or sell copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included
// in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT.
// IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY
// CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE
// SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
//////////////////////////////////////////////////////////////////////////
//
//%////////////////////////////////////////////////////////////////////////////


#include "UNIX_Common.h"

#include <sys/utsname.h>
#include <sys/stat.h>
#include <unistd.h>

String CIMHelper::getHostName()
{
     char    hostName[PEGASUS_MAXHOSTNAMELEN + 1];
     if (gethostname(hostName, sizeof(hostName)) != 0)
     {
         return String("localhost"); /* WTF? */
     }
     hostName[sizeof(hostName)-1] = 0;
     // Now get the official hostname.  If this call fails then return
     // the value from gethostname().
     /*
     struct  hostent *he;
     if ((he=gethostbyname(hostName)))
     {
         csName.assign(he->h_name);
     }
     else
     {
         csName.assign(hostName);
     }
     */
     return String(hostName);
}


/*
================================================================================
NAME              : getOSName
DESCRIPTION       : Platform-specific method to get OSname.
ASSUMPTIONS       : None
PRE-CONDITIONS    :
POST-CONDITIONS   :
NOTES             :
================================================================================
*/
String CIMHelper::getOSName()
{
    struct utsname unameInfo;
    // Call uname and check for any errors.
    if (uname(&unameInfo) < 0)
    {
       return String("");
    }
    return String(unameInfo.sysname);
}

CIMDateTime CIMHelper::getInstallDate()
{
	struct tm* clock;			// create a time structure
	struct stat attrib;			// create a file attribute structure
	stat("/mnt", &attrib);		// get the attributes mnt
	clock = gmtime(&(attrib.st_birthtime));	// Get the last modified time and put it into the time structure
	return CIMDateTime(
		clock->tm_year + 1900,
		clock->tm_mon + 1,
		clock->tm_mday,
		clock->tm_hour,
		clock->tm_min,
		clock->tm_sec,
		0,0,
		clock->tm_gmtoff);
}

CIMDateTime CIMHelper::getCurrentTime()
{
  time_t t = time(0);
  struct tm* clock;			// create a time structure
  clock = gmtime(&(t));	// Get the last modified time and put it into the time structure
  return CIMDateTime(
  	clock->tm_year + 1900, 
	clock->tm_mon + 1, 
	clock->tm_mday,
	clock->tm_hour,
	clock->tm_min,
	clock->tm_sec,
	0,0,
	clock->tm_gmtoff	
  );
}

String CIMHelper::getTimeAsString()
{
	return getCurrentTime().toString();
}

String CIMHelper::HostName(CIMHelper::getHostName());
String CIMHelper::OSName(CIMHelper::getOSName());
String CIMHelper::EmptyString(String::EMPTY);

#if defined(PEGASUS_OS_HPUX)

#elif defined(PEGASUS_OS_LINUX)

#elif defined(PEGASUS_OS_DARWIN)

#elif defined(PEGASUS_OS_AIX)

#elif defined(PEGASUS_OS_FREEBSD)
	kvm_t * CIMHelper::kd;
#elif defined(PEGASUS_OS_SOLARIS)

#elif defined(PEGASUS_OS_ZOS)

#elif defined(PEGASUS_OS_VMS)

#elif defined(PEGASUS_OS_TRU64)

#else

#endif