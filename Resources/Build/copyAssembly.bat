if NOT EXIST %1 mkdir %1

if %2 == "Release" (
xcopy %3 %1\*.*  /y )
