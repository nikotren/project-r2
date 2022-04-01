import urlSlug from 'url-slug';
import { jsPDF } from 'jspdf';

export default {
  methods: {
    saveImagePng: function() {
      var a  = document.createElement('a');
      let svg = 'data:image/svg+xml;base64,' + this.result;

      this.convertSvgToPng(svg, 2048, 2048, (data) => {
          a.href = data;
          if(this.actual_command.name) {
            a.download = urlSlug(this.actual_command.name) + '_' + String(+ new Date()) + '.png';
          }else{
            a.download = 'projectR_' + String(+ new Date()) + '.png';
          }
          a.click();
      });
    },
    saveImageSvg: function() {
        var a  = document.createElement('a');
        a.href = 'data:image/svg+xml;base64,' + this.result;
        if(this.actual_command.name) {
          a.download = urlSlug(this.actual_command.name) + '_' + String(+ new Date()) + '.svg';
        }else{
          a.download = 'projectR_' + String(+ new Date()) + '.svg';
        }
        a.click();
    },
    convertSvgToPng: async function(svgData, width, height, callback) {
        var canvas = document.createElement('canvas');
        let context = canvas.getContext('2d');
        canvas.width = width;
        canvas.height = height;

        let image = new Image();

        image.onload = function () {
            context.clearRect(0, 0, width, height);
            context.drawImage(image, 0, 0, width, height);
            let pngData = canvas.toDataURL('image/png');
            callback(pngData);
        }

        image.src = svgData;
    },
    saveImagePdf: function() {
        let doc = new jsPDF('p', 'pt', 'a4');
        let name = (this.actual_command.name ? urlSlug(this.actual_command.name) : 'projectR') + '_' + String(+ new Date()) + '.pdf';
        let svg = 'data:image/svg+xml;base64,' + this.result;
        this.convertSvgToPng(svg, 2048, 2048, (data) => {
            let w = doc.internal.pageSize.getWidth();
            doc.addImage(data, 0, 0, w, w);
            doc.save(name);
        });
    },
  }
};