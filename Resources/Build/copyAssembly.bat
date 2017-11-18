if NOT EXIST %1dist  mkdir %1dist 

if %2 == "Release" (
xcopy %3 %1dist\*.*  /y )
