import React, { Component } from 'react';
import { withStyles } from '@material-ui/core/styles';
import Webcam from 'react-webcam';

const styles = {
};

class Scan extends Component {
    render() {
        const { classes } = this.props;

        const videoConstraints = {
            facingMode: 'environment',
        };

        return (
            <Webcam
                audio={false}
                ref={this.setRef}
                screenshotFormat="image/jpeg"
                width={350}
                videoConstraints={videoConstraints}
                style={{
                    width: '100vw',
                }}
            />
        );
    }
  }
  
  export default withStyles(styles)(Scan);