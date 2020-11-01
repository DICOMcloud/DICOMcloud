sudo docker build --rm -t dicomcloud/dicomcloud ../ -f ../Development.Dockerfile
sudo docker run --rm -it -p 5021:80 dicomcloud/dicomcloud --name dicom-cloud-api-test