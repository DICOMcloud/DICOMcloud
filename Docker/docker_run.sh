sudo docker build --rm -t dicomcloud/dicomcloud ../ -f ../Dockerfile
sudo docker run --rm -it -p 5021:80 dicom-cloud-api --name dicom-cloud-api-test