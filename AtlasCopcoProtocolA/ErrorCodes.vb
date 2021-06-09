'*
'* Copyright (c) 2021 Manufacturing Automation, LLC
'*
'*  This library Is free software; you can redistribute it And/Or
'*  modify it under the terms Of the GNU Lesser General Public
'*  License as published by the Free Software Foundation; either
'*  version 2.1 Of the License, Or (at your option) any later version.
'*  See the file COPYING included With this distribution For more
'*  information.

Public Module ErrorCodes
    '* Dictionary for Error IDs *

    Public ReadOnly Errors As Dictionary(Of Integer, String) = New Dictionary(Of Integer, String) From {
   {0, "Reserved"},
   {1, "Invalid data"},
   {2, "Pset number not present"},
   {3, "Pset can not be set"},
   {4, "Pset not running"},
   {5, ""},
   {6, "VIN upload subscription already exists"},
   {7, "VIN upload subscription does not exist"},
   {8, "VIN input source not granted"},
   {9, "Last tightening result subscription already exists"},
   {10, "Last tightening result subscription does not exist"},
   {11, "Alarm subscription already exists"},
   {12, "Alarm subscription does not exist"},
   {13, "Parameter set selection subscription already exists"},
   {14, "Parameter set selection subscription does not exists"},
   {15, "Tightening ID requested not found"},
   {16, "Connection rejected protocol busy"},
   {17, "Job number not preset"},
   {18, "Job info subscription already exists"},
   {19, "Job info subscription does not exist"},
   {20, "Job can not be set"},
   {21, "Job not running"},
   {30, "Controller is not a sync Master"},
   {31, "Multi spindle status subscription already exists"},
   {32, "Multi spindle status subscription does not exist"},
   {33, "Multi spindle result subscription already exists"},
   {34, "Multi spindle result subscription does exist"},
   {40, "Job line control info subscription already exists"},
   {41, "Job line control info subscription does not exist"},
   {42, "Identifier input source not granted"},
   {43, "Multiple identifiers work order subscription already exists"},
   {44, "Multiple identifiers work order subscription does not exist"},
   {58, "No alarm present"},
   {59, "Tool currently in use"},
   {96, "Client already connected"},
   {97, "MID revision unsupported"},
   {98, "Controller internal request timeout"},
   {99, "Unknown MID"}
   }
End Module
